using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerController : MonoBehaviour
{
    public Transform playerCam;
    public Transform playerT;

    private float cameraPitch;
    public float sensitivity;
    public bool lockCursor = true;
    public bool paused;

    public AudioClip gunshot;
    public AudioSource weaponSound;

    public float baseSpeed;
    private float speed;
    private float moveSmoothTime = 0.1f;
    private Vector2 currentDirection;
    private Vector2 currentVelocity;
    private CharacterController character;

    public float fireRate;
    private float fireTimer;
    public Transform bulletOrigin;
    private LineRenderer laserLine;
    public Camera fpsCam;   
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);
    public float weaponRange;

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
        mouseControl();
        movementControl();
        fire();
    }
    void mouseControl()
    {
        Vector2 mousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        cameraPitch -= mousePosition.y * sensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -60.0f, 60.0f);

        playerCam.localEulerAngles = Vector3.right * cameraPitch;
        playerT.Rotate(Vector3.up * mousePosition.x * sensitivity);

    }
    void movementControl()
    {
        Vector2 targetDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDirection.Normalize();
        currentDirection = Vector2.SmoothDamp(currentDirection, targetDirection, ref currentVelocity, moveSmoothTime);

        if (Input.GetKey("left shift"))
        {
            speed = baseSpeed * 3f;
        }
        else
        {
            speed = baseSpeed;
        }
        Vector3 velocity = (playerT.forward * currentDirection.y + playerT.right * currentDirection.x) * speed;
        character.Move(velocity * Time.deltaTime);
    }
    void fire()
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
    private IEnumerator ShotEffect()
    {
        weaponSound.PlayOneShot(gunshot);
        laserLine.enabled = true;
        yield return shotDuration;
        laserLine.enabled = false;
    }
}
