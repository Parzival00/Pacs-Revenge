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
    private LineRenderer laserLine;
    private float fireTimer;

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
    }

    // Update is called once per frame
    void Update()
    {
        MouseControl();
        MovementControl();
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
        Vector3 velocity = (playerT.forward * currentDirection.y + playerT.right * currentDirection.x) * speed;
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
            /*Projectile bullet = Instantiate(Resources.Load<Projectile>("Bullet"), bulletOrigin.transform, false);
            bullet.transform.localEulerAngles = Vector3.up * -cameraPitch;
            bullet.transform.localPosition += Vector3.forward * 1.5f;
            bullet.transform.parent = null;*/
            StartCoroutine(ShotEffect());
            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
            RaycastHit hit;
            laserLine.SetPosition(0, bulletOrigin.position);
            if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange))
            {
                laserLine.SetPosition(1, hit.point);
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
}
