using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ghost : MonoBehaviour
{
    public enum Mode
    {
        Dormant,
        Chase,
        Scatter,
        Respawn
    }

    protected Mode currentMode;

    [SerializeField] protected NavMeshAgent navMesh;
    [SerializeField] protected Map map;
    [SerializeField] protected Transform ghostIcon;
    [SerializeField] protected GhostSpriteController spriteController;
    [SerializeField] protected SpriteRenderer spriteRenderer;

    [Header("Ghost Settings")]
    [SerializeField] protected int pelletsNeededToStart = 10;
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected float respawnWaitTime = 5f;

    [Header("Collider")]
    [SerializeField] protected Collider ghostCollider;

    [Header("Transform Targets")]
    [SerializeField] protected Transform player;
    [SerializeField] protected Transform scatterTarget;
    [SerializeField] protected Transform respawnPoint;

    protected Vector3 targetPosition;
    protected Vector2Int lastPlayerGridPosition;

    protected Vector2Int lastGridPosition;
    protected Vector2Int currentGridPosition;

    // Start is called before the first frame update
    void Start()
    {
        currentMode = Mode.Dormant;

        if (navMesh == null)
            navMesh = GetComponent<NavMeshAgent>();

        navMesh.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        currentGridPosition = map.GetGridLocation(transform.position);

        Act();
        RotateGhostIcons();

        lastGridPosition = currentGridPosition;
    }

    public void Act()
    {
        switch(currentMode)
        {
            case Mode.Chase:
                Chase();
                break;
            case Mode.Scatter:
                Scatter();
                break;
            case Mode.Dormant:
                Dormant();
                break;
            case Mode.Respawn:
                Respawn();
                break;
        }
    }

    //Chase the player
    protected virtual void Chase()
    {
        Vector2Int playerGridPosition = map.CheckEdgePositions(transform.position);

        if(lastPlayerGridPosition != playerGridPosition || !navMesh.enabled)
        {
            targetPosition = map.GetWorldFromGrid(playerGridPosition);

            navMesh.enabled = true;

            navMesh.SetDestination(targetPosition);
        }

        lastPlayerGridPosition = playerGridPosition;
    }

    //Scatter and move to the provided transforms location
    protected virtual void Scatter()
    {
        Vector2Int scatterTargetGridPos = map.GetGridLocation(scatterTarget.position);

        navMesh.SetDestination(map.GetWorldFromGrid(scatterTargetGridPos));
    }

    //Mode at the games start to keep ghost unactive until the required pellets are collected
    protected virtual void Dormant()
    {
        if(Score.pelletsCollected >= pelletsNeededToStart)
            currentMode = Mode.Chase;
    }

    bool startedRespawnSequence = false; //Used in the Respawn mode to check whether the timer has started

    //Mode activated when a ghost is shot. Ghost moves to provided respawn location and wants specified amount of time before shifting to Chase mode
    protected virtual void Respawn()
    {
        if (!startedRespawnSequence)
        {
            StartCoroutine(RespawnSequence());
        }
    }

    protected virtual IEnumerator RespawnSequence()
    {
        spriteRenderer.color = Color.black;
        navMesh.ResetPath();
        ghostCollider.enabled = false;
        startedRespawnSequence = true;

        WaitForSeconds deathWait = new WaitForSeconds(3f);

        yield return deathWait;

        Vector2Int respawnPointGridPos = map.GetGridLocation(respawnPoint.position);
        navMesh.SetDestination(map.GetWorldFromGrid(respawnPointGridPos));

        yield return null;

        WaitUntil arrivedAtRespawnPoint = new WaitUntil(() => navMesh.remainingDistance <= 0.1f);

        yield return new WaitUntil(() => navMesh.remainingDistance <= 0.1f);

        WaitForSeconds respawnWait = new WaitForSeconds(respawnWaitTime);

        yield return respawnWait;

        //Reset variables
        startedRespawnSequence = false;

        spriteRenderer.color = Color.white;

        currentMode = Mode.Chase;
        ghostCollider.enabled = true;
        lastPlayerGridPosition = new Vector2Int(-1, -1);
    }

    public virtual void InitiateScatter()
    {
        currentMode = Mode.Scatter;
    }
    public virtual void DeactivateScatter()
    {
        if(currentMode == Mode.Scatter)
            currentMode = Mode.Chase;
    }

    /// <summary>
    /// Called when ghost is hit with the gun and sets the mode to respawn
    /// </summary>
    public virtual void GotHit()
    {
        currentMode = Mode.Respawn;
    }

    /// <summary>
    /// Disable the nav mesh temporarily to set the position to given location. (Used in combination with the teleport class) 
    /// </summary>
    public void SetPosition(Vector3 pos)
    {
        navMesh.enabled = false;
        transform.position = pos;
    }

    private void RotateGhostIcons()
    {
        ghostIcon.rotation = Quaternion.Euler(90, player.transform.eulerAngles.y, 0);
    }
}
