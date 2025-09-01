using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] PlayerStateManager playerState;
    [SerializeField] PlayerSettings playerSettings;

    [Header("Player References")]
    [SerializeField] Transform playerCam;
    [SerializeField] Transform playerT;
    [SerializeField] CharacterController character;

    [Header("Movement Settings")]
    [SerializeField] float baseSpeed;
    [SerializeField] float sprintMultiplier;
    [SerializeField] float gunSpeedMultiplier;
    [SerializeField] Transform playerSpawnPoint;
    private float speed;
    private float moveSmoothTime = 0.1f;
    private Vector2 currentDirection;
    private Vector2 currentVelocity;
    private Vector3 velocity = Vector3.zero;
    public Vector3 Velocity => velocity;  //used for view bobbing as well

    [Header("Mouse Settings")]
    [SerializeField] bool paused;
    [SerializeField] float sensitivity;
    float cameraPitch;
    float fov;

    private float originalY;

    //States
    bool canMove = true;
    bool canLook = true;

    private void Start()
    {
        originalY = transform.position.y;
    }

    private void Update()
    {
        if(canMove)
        {
            MovementControl();
        }

        if(canLook)
        {
            MouseControl();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        targetDirection = context.ReadValue<Vector2>().normalized;
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>() * 0.5f;
    }

    Vector2 mousePosition;
    /// <summary>
    /// This method takes the mouse position and rotates the player and camera accordingly. Using the x position of the mouse for horizontal and the y position for vertical.
    /// </summary>
    void MouseControl()
    {
        cameraPitch -= mousePosition.y * sensitivity * Mathf.Min(0.01f, Time.deltaTime) * 30f * Time.timeScale;
        cameraPitch = Mathf.Clamp(cameraPitch, -35.0f + (fov - 70) / 1.5f, 50.0f);

        playerCam.localEulerAngles = Vector3.right * cameraPitch;
        playerT.Rotate(Vector3.up * mousePosition.x * sensitivity * Mathf.Min(0.01f, Time.deltaTime) * 30f * Time.timeScale);
    }

    Vector2 targetDirection;
    /// <summary>
    /// Uses the input axis system to move the player's character controller
    /// Has a sprint ability activated by the L-shift key
    /// </summary>
    void MovementControl()
    {
        currentDirection = Vector2.SmoothDamp(currentDirection, targetDirection, ref currentVelocity, moveSmoothTime);

        speed = baseSpeed;

        velocity = (playerT.forward * currentDirection.y + playerT.right * currentDirection.x) * speed;
        character.enabled = true;
        character.Move(velocity * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, originalY, transform.position.z); //Ensure character stays on the ground
    }


    //Event Handlers
    private void HandleMoveState(bool canMove)
    {
        this.canMove = canMove;
        canLook = canMove;
    }
    private void HandleTrappedState(bool isTrapped)
    {
        canMove = isTrapped;
    }
    private void HandleGunState(bool gunActivated)
    {
        if(gunActivated)
        {
            baseSpeed += gunSpeedMultiplier;
        }
        else
        {
            baseSpeed -= gunSpeedMultiplier;
        }
    }

    private void HandleSettingsChanged(PlayerSettings.SettingType type)
    {
        if(type == PlayerSettings.SettingType.FOV)
        {
            fov = playerSettings.FOV;
        }

        if(type == PlayerSettings.SettingType.Sensitivity)
        {
            sensitivity = playerSettings.Sensitivity;
        }
    }

    //Subscribe and Unsubscribe to events
    private void OnEnable()
    {
        playerState.OnMoveStateChanged += HandleMoveState;
        playerState.OnTrappedStateChanged += HandleTrappedState;
        playerState.OnGunStateChanged += HandleGunState;

        playerSettings.OnSettingsChanged += HandleSettingsChanged;
        fov = playerSettings.FOV;
        sensitivity = playerSettings.Sensitivity;
    }

    private void OnDisable()
    {
        playerState.OnMoveStateChanged -= HandleMoveState;
        playerState.OnTrappedStateChanged -= HandleTrappedState;
        playerState.OnGunStateChanged -= HandleGunState;

        playerSettings.OnSettingsChanged -= HandleSettingsChanged;
    }
}
