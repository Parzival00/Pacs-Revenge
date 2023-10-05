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
    public bool canMove { get; private set; }

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
    [SerializeField] RailgunVFXController railGunVFX;
    [SerializeField] Animator gunAnimator;
    [SerializeField] float chargeTime;
    [SerializeField] float maxWeaponTemp = 5;
    [SerializeField] float overheatSpeed = 1;
    [SerializeField] float cooldownSpeed = 1;
    [SerializeField] float dechargeTime = .25f;
    [SerializeField] float gunTimeAmount = 5f;
    [SerializeField] Transform bulletOrigin;
    [SerializeField] Camera fpsCam;
    [SerializeField] WaitForSeconds shotDuration = new WaitForSeconds(0.07f);
    [SerializeField] float weaponRange;
    [SerializeField] LayerMask targetingMask;

    private LineRenderer laserLine;
    private float weaponCharge;
    private float weaponDecharge;
    private float weaponTemp;
    private bool overheated = false;
    private bool weaponDecharging = false;

    public float WeaponCharge { get => weaponCharge; }
    public float WeaponDecharge { get => weaponDecharge; }
    public float WeaponTemp01 { get => (weaponTemp / maxWeaponTemp); }
    public bool Overheated { get => overheated; }
    public bool WeaponDecharging { get => weaponDecharging; }

    private float gunTimer;
    public float GunTimer01 { get => (gunTimer / gunTimeAmount); }
    
    [Header("GameObject Refereneces")]
    [SerializeField] GameObject gun;
    [SerializeField] GameObject hud;
    [SerializeField] Image fadeImage;
    [SerializeField] Text LivesText;

    Ghost[] ghosts;

    [Header("Player Settings")]
    [SerializeField] float deathSequenceLength = 1;
    [SerializeField] int playerLives = 3;

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
    private bool canFire = true;

    private bool inDeathSequence;

    private float originalY;

    // Start is called before the first frame update
    void Start()
    {
        Init();
        musicPlayer.PlayOneShot(bgMusic);
    }

    private void Init()
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

        gunActivated = false;
        gun.SetActive(false);
        hud.SetActive(false);

        Time.timeScale = 1;

        fadeImage.canvasRenderer.SetAlpha(0.01f);

        if (animator == null)
            animator = GetComponent<Animator>();

        ghosts = new Ghost[4];
        ghosts = FindObjectsOfType<Ghost>();

        if (usePlayerPrefsSettings)
        {
            ApplyGameSettings();
        }

        canMove = true;
        canFire = true;

        LivesText.text = "Lives: " + playerLives;
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
            CheckWeaponTemp();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            gameIsPaused = !gameIsPaused;
            PauseGame();
        }
    }

    #region Move and Look
    /// <summary>
    /// This method takes the mouse position and rotates the player and camera accordingly. Using the x position of the mouse for horizontal and the y position for vertical.
    /// </summary>
    void MouseControl()
    {
        Vector2 mousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        cameraPitch -= mousePosition.y * sensitivity * Time.deltaTime * 100f * Time.timeScale;
        cameraPitch = Mathf.Clamp(cameraPitch, -60.0f, 60.0f);

        playerCam.localEulerAngles = Vector3.right * cameraPitch;
        playerT.Rotate(Vector3.up * mousePosition.x * sensitivity * Time.deltaTime * 100f * Time.timeScale);

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
    #endregion

    #region Gun Functionality
    /// <summary>
    /// When the left mouse button is pressed this generates a raycast to where the player was aiming.
    /// A sound effect is played and the raytrace is rendered
    /// </summary>
    void Fire()
    {
        if (canFire == false)
            return;

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

                weaponTemp = 0f;

                StartCoroutine(Decharge());
            }
        }
        else if (!paused && Input.GetMouseButton(0) && !overheated)
        {
            if (weaponCharge < 1f)
            {
                weaponCharge += (Time.deltaTime * (1 / chargeTime));
            }
            weaponTemp += (Time.deltaTime * (overheatSpeed));
        } else if (!Input.GetMouseButton(0) && !overheated)
        {
            if(weaponCharge > 0)
            {
                weaponCharge -= (Time.deltaTime * (1 / (chargeTime / 2)));
                weaponTemp -= Time.deltaTime * (overheatSpeed * 2);
            } else
            {
                weaponTemp = 0;
                weaponCharge = 0;
            }
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
    /// Decharges the gun and acts as a shot cooldown
    /// </summary>
    /// <returns></returns>
    private IEnumerator Decharge()
    {
        canFire = false;
        weaponCharge = 1;
        weaponDecharge = 0;

        weaponDecharging = true;

        while (weaponDecharge <= 1)
        {
            weaponCharge -= (Time.deltaTime * (1 / dechargeTime));
            weaponDecharge += (Time.deltaTime * (1 / dechargeTime));

            yield return null;
        }

        weaponDecharging = false;

        weaponCharge = 0;
        weaponDecharge = 1;
        canFire = true;
    }

    /// <summary>
    /// Handles the changes to the charge UI and the sound effect for weapon overheating. Also decreases the temperature over time
    /// </summary>
    void CheckWeaponTemp()
    {
        if (weaponTemp >= maxWeaponTemp && !overheated)
        {
            overheated = true;
            weaponSound.volume = .9f;
            weaponSound.PlayOneShot(overheat);
            weaponSound.volume = .1f;
        }
        else if (weaponTemp <= 0f && overheated)
        {
            weaponTemp = 0f;
            weaponCharge = 0f;
            overheated = false;
        }
        else if (overheated && !Input.GetMouseButton(0))
        {
            weaponTemp -= Time.deltaTime * cooldownSpeed;
        }
    }
    #endregion

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

    #region Gun Powerup Functions
    Coroutine gunTimerCoroutine;
    /// <summary>
    /// Activates the gun and any related visuals and start the gun timer coroutine
    /// </summary>
    public void ActivateGun()
    {
        if (gunTimerCoroutine != null)
            StopCoroutine(gunTimerCoroutine);

        gunActivated = true;
        gun.SetActive(true);
        hud.SetActive(true);

        musicPlayer.Stop();
        musicPlayer.Play();

        if (gunAnimator != null)
        {
            gunAnimator.ResetTrigger("UnequipGun");
            gunAnimator.SetTrigger("EquipGun");
        }

        weaponTemp = 0;
        weaponCharge = 0;
        weaponDecharge = 0;
        overheated = false;
        weaponDecharging = false;

        if (railGunVFX != null)
            railGunVFX.ActivateEffects();

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

        gunTimer = gunTimeAmount;

        while(gunTimer >= 0)
        {
            gunTimer -= Time.deltaTime;
            yield return null;
        }

        DeactivateGun();
        musicPlayer.PlayOneShot(bgMusic);
        gunTimer = gunTimeAmount;
    }

    /// <summary>
    /// Deactivates the gun and any related visuals
    /// </summary>
    void DeactivateGun()
    {
        gunActivated = false;
        //if(animator == null)
        gun.SetActive(false);
        hud.SetActive(false);

        if (gunAnimator != null)
        {
            gunAnimator.ResetTrigger("EquipGun");
            gunAnimator.SetTrigger("UnequipGun");
        }

        foreach (Ghost ghost in ghosts)
        {
            ghost.DeactivateScatter();
        }

        gunTimerCoroutine = null;

        //Deactivate any remaining target outline
        targetOutlineController.DeactivateOutline();
    }
    #endregion

    #region Collision Detection (Weapon, Fruit, Enemy)
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Weapon" && !gunActivated)
        {
            Destroy(other.gameObject);
            ActivateGun();
        }
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

                if(!inDeathSequence)
                {
                    StartCoroutine(DeathSequence());
                }
            }
        }
    }
    #endregion

    IEnumerator DeathSequence() 
    {
        inDeathSequence = true;

        canMove = false;
        character.enabled = false;

        //Freeze all of the ghosts' movements
        foreach (Ghost g in ghosts)
        {
            g.FreezeGhost();
        }

        fadeImage.canvasRenderer.SetAlpha(0f);
        fadeImage.gameObject.SetActive(true);

        //Play Player death animation
        animator.SetTrigger("Death");

        WaitForSecondsRealtime deathTimer = new WaitForSecondsRealtime(deathSequenceLength / 2);

        yield return deathTimer;

        fadeImage.CrossFadeAlpha(255f, 100f, false);

        yield return deathTimer;

        //Reset the Ghosts
        foreach (Ghost g in ghosts)
        {
            g.ResetGhost();
        }

        //Reset the fruit (just in case)
        FruitController fruitController = FindObjectOfType<FruitController>();
        if (fruitController != null)
        {
            fruitController.DeactivateFruit();
        }

        transform.position = playerSpawnPoint.position;

        playerLives--;

        if (playerLives <= 0)
        {
            print("Ending Scene");
            //end game scene
            SceneManager.LoadScene(2);
        }
        else
        {
            fadeImage.gameObject.SetActive(false);

            animator.ResetTrigger("Death");
            animator.SetTrigger("Respawn");

            yield return deathTimer;

            character.enabled = true;
            canMove = true;

            inDeathSequence = false;

            LivesText.text = "Lives: " + playerLives;

            animator.ResetTrigger("Respawn");
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

    #region UI Functions
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
    public void ApplyGameSettings()
    {
        if (PlayerPrefs.HasKey("FOV"))
            playerCam.GetComponent<Camera>().fieldOfView = PlayerPrefs.GetFloat("FOV");
        
        if(PlayerPrefs.HasKey("Volume"))
            AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        
        if (PlayerPrefs.HasKey("Sensitivity"))
            sensitivity = PlayerPrefs.GetFloat("Sensitivity");
    }
    #endregion
}
