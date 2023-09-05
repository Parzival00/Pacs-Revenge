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

    public float baseSpeed;
    private float speed;
    private float moveSmoothTime = 0.1f;
    private Vector2 currentDirection;
    private Vector2 currentVelocity;
    private CharacterController character;

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
    }

    // Update is called once per frame
    void Update()
    {
        mouseControl();
        movementControl();
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
}
