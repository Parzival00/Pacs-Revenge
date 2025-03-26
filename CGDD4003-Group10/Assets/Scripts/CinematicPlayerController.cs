using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CinematicPlayerController : MonoBehaviour
{
    [SerializeField] Transform playerCam;
    [SerializeField] Transform playerT;
    [SerializeField] float moveSpeed;
    [SerializeField] float lookSpeed;
    [SerializeField] bool hideUI;
    [SerializeField] GameObject[] hideObjects;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MouseControl();
        MovementControl();

        if(hideUI)
        {
            for (int i = 0; i < hideObjects.Length; i++)
            {
                if (hideObjects[i] != null) hideObjects[i].SetActive(false);
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        targetDirection = context.ReadValue<Vector2>().normalized;
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>() * 0.1f;
    }

    float cameraPitch;
    Vector2 mousePosition;
    private Vector2 currentDirection;
    private Vector2 currentVelocity;
    private float moveSmoothTime = 0.1f;

    /// <summary>
    /// This method takes the mouse position and rotates the player and camera accordingly. Using the x position of the mouse for horizontal and the y position for vertical.
    /// </summary>
    void MouseControl()
    {

        cameraPitch -= mousePosition.y * lookSpeed * Mathf.Min(0.01f, Time.deltaTime) * 30f * Time.timeScale;
        cameraPitch = Mathf.Clamp(cameraPitch, -35.0f, 50.0f);

        playerCam.localEulerAngles = Vector3.right * cameraPitch;
        playerT.Rotate(Vector3.up * mousePosition.x * lookSpeed * Mathf.Min(0.01f, Time.deltaTime) * 30f * Time.timeScale);

    }

    Vector2 targetDirection;
    /// <summary>
    /// Uses the input axis system to move the player's character controller
    /// Has a sprint ability activated by the L-shift key
    /// </summary>
    void MovementControl()
    {
        currentDirection = Vector2.SmoothDamp(currentDirection, targetDirection, ref currentVelocity, moveSmoothTime);

        float speed = moveSpeed;

        Vector3 velocity = (playerCam.forward * currentDirection.y + playerCam.right * currentDirection.x) * speed;
        playerT.position += velocity * Time.deltaTime;
        //character.enabled = true;
        //character.Move(velocity * Time.deltaTime);

        //transform.position = new Vector3(transform.position.x, originalY, transform.position.z); //Ensure character stays on the ground
    }
}
