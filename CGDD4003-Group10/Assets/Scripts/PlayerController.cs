using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerController : MonoBehaviour
{
    public Transform playerCam;
    public Transform playerT;
    static bool gameIsPaused;
    public GameObject pauseMenu;
    [SerializeField] GameObject optionsMenu;
    private bool canMove;

    [Header("Movement Settings")]
    [SerializeField] float baseSpeed;
    [SerializeField] float sprintMultiplier;
    [SerializeField] Transform playerSpawnPoint;
    private float speed;
    private float moveSmoothTime = 0.1f;
    private Vector2 currentDirection;
    private Vector2 currentVelocity;
    private CharacterController character;
    [HideInInspector]
    public Vector3 velocity = Vector3.zero; //used for view bobbing as well

    float cameraPitch;
    [Header("Mouse Settings")]
    [SerializeField] bool lockCursor = true;
    [SerializeField] bool paused;
    [SerializeField] float sensitivity;

    [Header("Weapon Audio")]
    [SerializeField] AudioClip gunshot;
    [SerializeField] AudioClip overheat;
    [SerializeField] AudioSource weaponSound;

    [Header("Weapon Settings")]
    [SerializeField] float chargeTime;
    [SerializeField] float overheatTime;
    [SerializeField] float cooldown;
    [SerializeField] Transform bulletOrigin;
    [SerializeField] Camera fpsCam;
    [SerializeField] WaitForSeconds shotDuration = new WaitForSeconds(0.07f);
    [SerializeField] float weaponRange;
    [SerializeField] LayerMask targetingMask;
    [SerializeField] float gunTimeAmount = 5f;
    private LineRenderer laserLine;
    private float weaponCharge;
    private float weaponTemp;
    private bool overheated;
    Slider weaponChargeBar;
    Image chargeBarFill;

    private WaitForSeconds gunTimer;
    

    [Header("GameObject Refereneces")]
    [SerializeField] GameObject gun;
    [SerializeField] GameObject hud;
    [SerializeField] Image fadeImage;
    [SerializeField] Text LivesText;
    Ghost[] ghosts;

    [Header("Death Settings")]
    //camera fade variables
    [SerializeField] float fadeTimeModifier = 1;
    [SerializeField] int playerLives = 3;
    private bool hitable;
    private float hitTimer;

    [Header("Player Animator")]
    [SerializeField] Animator animator;

    [Header("Target Outline Controller")]
    [SerializeField] TargetOutlineController targetOutlineController;

    [Header("Music Settings")]
    [SerializeField] AudioClip powerMusic;
    [SerializeField] AudioClip bgMusic;
    [SerializeField] AudioClip gameStart;
    [SerializeField] AudioSource musicPlayer;

    [Header("Debug Settings")]
    [SerializeField] bool usePlayerPrefsSettings;

    public static bool gunActivated { get; private set; }

    private float originalY;

    // Start is called before the first frame update
    void Start()
    {
        originalY = transform.position.y;

        character = this.GetComponent<CharacterController>();
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            ResumeGame();
        }
        speed = baseSpeed;
        laserLine = GetComponent<LineRenderer>();
        this.weaponChargeBar = (Slider)GameObject.FindGameObjectWithTag("ChargeBar").GetComponent("Slider");
        this.chargeBarFill = (Image)GameObject.FindGameObjectWithTag("ChargeFill").GetComponent("Image");
        gunActivated = false;
        gun.SetActive(false);
        hud.SetActive(false);

        hitable = true;
        hitTimer = 0;
        fadeImage.canvasRenderer.SetAlpha(0.01f);

        gunTimer = new WaitForSeconds(gunTimeAmount);

        if (animator == null)
            animator = GetComponent<Animator>();

        ghosts = new Ghost[4];
        ghosts = FindObjectsOfType<Ghost>();

        if (usePlayerPrefsSettings)
        {
            applyGameSettings();
        }
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            MouseControl();
            MovementControl();
        }
        

        if (gunActivated)
        {
            Fire();
            OutlineTargetEnemy();
            UpdateChargeBar();
            checkWeaponTemp();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            gameIsPaused = !gameIsPaused;
            PauseGame();
        }

        if (!hitable)
        {
            hitTimer += Time.deltaTime;
        }
        if(hitTimer >= fadeTimeModifier && !hitable)
        {
            hitTimer = 0;
            hitable = true;
            if (playerLives < 1)
            {
                print("Ending Scene");
                fadeImage.CrossFadeAlpha(0.01f, 1, false);
                //end game scene
                SceneManager.LoadScene(2);
            }
            else
            {
                //reset player and ghosts
                transform.position = playerSpawnPoint.position;
                fadeImage.CrossFadeAlpha(0.01f, 1, false);
                canMove = true;
            }
        }
        LivesText.text = "Lives: " + playerLives;
    }

    /// <summary>
    /// This method takes the mouse position and rotates the player and camera accordingly. Using the x position of the mouse for horizontal and the y position for vertical.
    /// </summary>
    void MouseControl()
    {
        Vector2 mousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        cameraPitch -= mousePosition.y * sensitivity * Time.timeScale;
        cameraPitch = Mathf.Clamp(cameraPitch, -60.0f, 60.0f);

        playerCam.localEulerAngles = Vector3.right * cameraPitch;
        playerT.Rotate(Vector3.up * mousePosition.x * sensitivity * Time.timeScale);

    }

    /// <summary>
    /// Uses the input axis system to move the player's character controller
    /// Has a sprint ability activated by the L-shift key
    /// </summary>
    void MovementControl()
    {
        Vector2 targetDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDirection.Normalize();
        currentDirection = Vector2.SmoothDamp(currentDirection, targetDirection, ref currentVelocity, moveSmoothTime);

        if (Input.GetKey("left shift"))
        {
            speed = baseSpeed * sprintMultiplier;
        }
        else
        {
            speed = baseSpeed;
        }
        velocity = (playerT.forward * currentDirection.y + playerT.right * currentDirection.x) * speed;
        character.enabled = true;
        character.Move(velocity * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, originalY, transform.position.z); //Ensure character stays on the ground
    }

    /// <summary>
    /// When the left mouse button is pressed this generates a raycast to where the player was aiming.
    /// A sound effect is played and the raytrace is rendered
    /// </summary>
    void Fire()
    {
        if (Input.GetMouseButtonUp(0) && !paused && !overheated)
        {
            if (weaponCharge >= 1)
            {
                StartCoroutine(ShotEffect());
                Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
                RaycastHit hit;
                laserLine.SetPosition(0, bulletOrigin.position);

                //Detect hit on enemy
                if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange, targetingMask))
                {
                    laserLine.SetPosition(1, hit.point);

                    print("Hit: " + hit.collider.gameObject.name);

                    TargetAreaCollider targetAreaCollider = hit.collider.GetComponent<TargetAreaCollider>();

                    if (targetAreaCollider != null)
                    {
                        Ghost.HitInformation hitInformation = targetAreaCollider.OnShot();
                        Score.AddToScore(hitInformation.pointWorth + hitInformation.targetArea.pointsAddition);
                    }
                }
                else
                {
                    laserLine.SetPosition(1, rayOrigin + (fpsCam.transform.forward * weaponRange));
                }
            }
            weaponCharge = 0f;
            weaponTemp = 0f;
        }
        else if (!paused && Input.GetMouseButton(0) && !overheated)
        {
            if (weaponCharge < 1f)
            {
                weaponCharge += (Time.deltaTime * (1 / chargeTime));
            }
            weaponTemp += (Time.deltaTime * (overheatTime));
        }
    }
    /// <summary>
    /// Plays the weapon sound effect and enables the line render
    /// </summary>
    private IEnumerator ShotEffect()
    {
        weaponSound.PlayOneShot(gunshot);
        laserLine.enabled = true;
        yield return shotDuration;
        laserLine.enabled = false;
    }
    /// <summary>
    /// Updates the size of the charge bar based on current charge
    /// </summary>
    void UpdateChargeBar()
    {
        weaponChargeBar.value = weaponCharge;
    }
    /// <summary>
    /// Handles the changes to the charge UI and the sound effect for weapon overheating. Also decreases the temperature over time
    /// </summary>
    void checkWeaponTemp()
    {
        if (weaponTemp >= 5f && !overheated)
        {
            overheated = true;
            chargeBarFill.color = Color.red;
            weaponSound.volume = .9f;
            weaponSound.PlayOneShot(overheat);
            weaponSound.volume = .1f;
        }
        else if (weaponTemp <= 0f && overheated)
        {
            weaponTemp = 0f;
            weaponCharge = 0f;
            overheated = false;
            chargeBarFill.color = new Color(0f, 255f, 150f);
        }
        else if (overheated && !Input.GetMouseButton(0))
        {
            weaponTemp -= Time.deltaTime * cooldown;
        }
    }
    /// <summary>
    /// Activates the corresponding outline for targeted area
    /// </summary>
    private void OutlineTargetEnemy()
    {
        Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit hit;

        //Detect hit on enemy
        if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange, targetingMask))
        {
            //print("Targetting: " + hit.collider.gameObject.name);

            TargetAreaCollider targetAreaCollider = hit.collider.GetComponent<TargetAreaCollider>();

            if (targetAreaCollider != null)
            {
                SpriteRenderer outline = targetAreaCollider.OnTarget();
                targetOutlineController.SetTargetOutline(outline);
            }
        } 
        else
        {
            //print("Not Targetting Anything");
            targetOutlineController.DeactivateOutline();
        }
    }

    /// <summary>
    /// Disable the character controller temporarily to set the position to given location. (Used in combination with the teleport class) 
    /// </summary>
    public void SetPosition(Vector3 pos)
    {
        character.enabled = false;
        transform.position = pos;
    }

    Coroutine gunTimerCoroutine;
    /// <summary>
    /// Activates the gun and any related visuals and start the gun timer coroutine
    /// </summary>
    public void ActivateGun()
    {
        if (gunTimerCoroutine != null)
            StopCoroutine(gunTimerCoroutine);
        musicPlayer.Stop();
        musicPlayer.PlayOneShot(powerMusic);
        gunActivated = true;
        if(animator == null)
            gun.SetActive(true);
        hud.SetActive(true);

        if (animator != null)
        {
            animator.ResetTrigger("UnequipGun");
            animator.SetTrigger("EquipGun");
        }

        Ghost[] ghosts = FindObjectsOfType<Ghost>();
        foreach(Ghost ghost in ghosts)
        {
            ghost.InitiateScatter();
        }
        gunTimerCoroutine = StartCoroutine(GunTimer());
    }

    /// <summary>
    /// Waits a certain amount before deactivating the gun
    /// </summary>
    IEnumerator GunTimer()
    {
        yield return gunTimer;

        DeactivateGun();
    }

    /// <summary>
    /// Deactivates the gun and any related visuals
    /// </summary>
    void DeactivateGun()
    {
        gunActivated = false;
        if(animator == null)
            gun.SetActive(false);
        hud.SetActive(false);

        if (animator != null)
        {
            animator.ResetTrigger("EquipGun");
            animator.SetTrigger("UnequipGun");
        }

        Ghost[] ghosts = FindObjectsOfType<Ghost>();
        foreach (Ghost ghost in ghosts)
        {
            ghost.DeactivateScatter();
        }

        gunTimerCoroutine = null;

        //Deactivate any remaining target outline
        targetOutlineController.DeactivateOutline();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Weapon" && !gunActivated)
        {
            Destroy(other.gameObject);
            ActivateGun();
        }

        //if(other.tag == "Enemy")
        //{
        //    Ghost ghost = other.gameObject.transform.root.GetComponent<Ghost>();

        //    if (ghost.CurrentMode == Ghost.Mode.Chase)
        //    {
        //        //fade to black
        //        if (hitable)
        //        {
        //            playerLives--;
        //            hitable = false;
        //            fadeImage.CrossFadeAlpha(255, fadeTimeModifier, false);
        //        }
                
        //    }
        //}
    }

    //Accounting for scatter ending when player is touching a ghost
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Ghost ghost = other.gameObject.transform.root.GetComponent<Ghost>();

            if (ghost.CurrentMode == Ghost.Mode.Chase)
            {
                print("hit by " + other.gameObject.name);

                //fade to black
                if (hitable)
                {
                    playerLives--;
                    hitable = false;
                    canMove = false;
                    fadeImage.canvasRenderer.SetAlpha(0.01f);

                    fadeImage.CrossFadeAlpha(255f, 100f, false);

                    transform.position = playerSpawnPoint.position;

                    foreach (Ghost g in ghosts)
                    {
                        g.ResetGhost();
                    }

                    FruitController fruitController = FindObjectOfType<FruitController>();

                    if(fruitController != null)
                    {
                        fruitController.DeactivateFruit();
                    }
                }


            }
        }
    }

    ///Summary
    ///Pause Game Function
    ///Summary
    private void PauseGame() 
    {
        if (gameIsPaused)
        {
            Time.timeScale = 0.0f;
            AudioListener.pause = true;

            pauseMenu.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else 
        {
            ResumeGame();
        }
    }

    /// <summary>
    /// Resumes the game
    /// </summary>
    public void ResumeGame()
    {
        gameIsPaused = false;
        Time.timeScale = 1.0f;
        AudioListener.pause = false;
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// gets the players settings from playerprefs and applies them to the player
    /// will be also used for the pause menu
    /// </summary>
    public void applyGameSettings()
    {
        print(PlayerPrefs.GetFloat("FOV"));
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        playerCam.GetComponent<Camera>().fieldOfView = PlayerPrefs.GetFloat("FOV");
        sensitivity = PlayerPrefs.GetFloat("Sensitivity");
    }
}
