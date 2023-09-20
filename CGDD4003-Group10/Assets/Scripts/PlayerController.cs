using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerController : MonoBehaviour
{
    public Transform playerCam;
    public Transform playerT;

    [Header("Movement Settings")]
    [SerializeField] float baseSpeed;
    [SerializeField] float sprintMultiplier;
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

    [Header("Weapon Settings")]
    [SerializeField] AudioClip gunshot;
    [SerializeField] AudioSource weaponSound;
    [SerializeField] float fireRate;
    [SerializeField] Transform bulletOrigin;
    [SerializeField] Camera fpsCam;
    [SerializeField] WaitForSeconds shotDuration = new WaitForSeconds(0.07f);
    [SerializeField] float weaponRange;
    [SerializeField] LayerMask targetingMask;
    [SerializeField] float gunTimeAmount = 5f;
    private LineRenderer laserLine;
    private float fireTimer;

    private WaitForSeconds gunTimer;

    [Header("GameObject Refereneces")]
    [SerializeField] GameObject gun;
    [SerializeField] GameObject hud;
    [SerializeField] GameObject railgunChargeBar;

    private bool gunActivated;

    // Start is called before the first frame update
    void Start()
    {
        character = this.GetComponent<CharacterController>();
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        speed = baseSpeed;
        laserLine = GetComponent<LineRenderer>();

        gunActivated = true;
        gun.SetActive(true);
        hud.SetActive(true);

        gunTimer = new WaitForSeconds(gunTimeAmount);
    }

    // Update is called once per frame
    void Update()
    {
        MouseControl();
        MovementControl();

        if(gunActivated)
            Fire();
    }
    /// <summary>
    /// This method takes the mouse position and rotates the player and camera accordingly. Using the x position of the mouse for horizontal and the y position for vertical.
    /// </summary>
    void MouseControl()
    {
        Vector2 mousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        cameraPitch -= mousePosition.y * sensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -60.0f, 60.0f);

        playerCam.localEulerAngles = Vector3.right * cameraPitch;
        playerT.Rotate(Vector3.up * mousePosition.x * sensitivity);

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
    }
    /// <summary>
    /// When the left mouse button is pressed this generates a raycast to where the player was aiming.
    /// A sound effect is played and the raytrace is rendered
    /// </summary>
    void Fire()
    {
        if (fireTimer <= 0 && Input.GetMouseButton(0) && !paused)
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

                if(targetAreaCollider != null)
                {
                    Ghost.HitInformation hitInformation = targetAreaCollider.OnShot();
                    Score.AddToScore(hitInformation.pointWorth + hitInformation.targetArea.pointsAddition);
                }
            }
            else
            {
                laserLine.SetPosition(1, rayOrigin + (fpsCam.transform.forward * weaponRange));
            }
            fireTimer = fireRate;
            //weaponSound.PlayOneShot(gunshot);
        }
        else if (fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;
        }
    }
    /// <summary>
    /// Plays the weapon sound effect and enables the line render
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShotEffect()
    {
        weaponSound.PlayOneShot(gunshot);
        laserLine.enabled = true;
        yield return shotDuration;
        laserLine.enabled = false;
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

        gunActivated = true;
        gun.SetActive(true);
        hud.SetActive(true);

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
        gun.SetActive(false);
        hud.SetActive(false);

        Ghost[] ghosts = FindObjectsOfType<Ghost>();
        foreach (Ghost ghost in ghosts)
        {
            ghost.DeactivateScatter();
        }

        gunTimerCoroutine = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Weapon")
        {
            Destroy(other.gameObject);
            ActivateGun();
        }
    }
}
