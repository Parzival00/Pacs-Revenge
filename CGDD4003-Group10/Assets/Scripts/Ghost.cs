using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ghost : MonoBehaviour
{
    [System.Serializable]
    public struct TargetArea
    {
        public float respawnTimeAddition;
        public int pointsAddition;
        public TargetAreaType type;
    }

    public struct HitInformation
    {
        public TargetArea targetArea;
        public int pointWorth;
    }

    public enum Mode
    {
        Dormant,
        Exiting,
        Chase,
        Scatter,
        Respawn
    }

    public enum TargetAreaType
    {
        Head,
        Tentacles,
        Body,
        Mouth,
        LeftEye,
        RightEye
    }

    protected Mode currentMode;

    [SerializeField] protected NavMeshAgent navMesh;
    [SerializeField] protected Map map;
    [SerializeField] protected Transform ghostIcon;
    [SerializeField] protected GhostSpriteController spriteController;
    [SerializeField] protected SpriteRenderer spriteRenderer;

    [Header("Ghost Settings")]
    [SerializeField] protected bool exitSpawnToRight;
    [SerializeField] protected int pelletsNeededToStart = 10;
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected float respawnWaitTime = 5f;
    [SerializeField] protected int pointWorth = 20;
    [SerializeField] protected TargetArea[] targetAreas;

    [Header("Transform Targets")]
    [SerializeField] protected Transform player;
    [SerializeField] protected Transform scatterTarget;
    [SerializeField] protected Transform respawnPoint;
    [SerializeField] protected Transform spawnExit;

    [Header("Player Dectecting Collider")]
    [SerializeField] Collider ghostCollider;

    [Header("Sound Settings")]
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip chaseSound;
    [SerializeField] AudioClip scatterSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip corpseMoveSound;
    [SerializeField] AudioClip respawnedSound;

    //AI Variables
    protected Vector3 targetPosition;
    protected Vector2Int targetGridPosition;
    protected Vector2Int lastTargetGridPosition;

    protected Vector2Int lastGridPosition;
    protected Vector2Int currentGridPosition;
    protected Vector2Int nextGridPosition;

    protected Vector2Int currentDirection;

    //Target Area Variables
    protected Dictionary<TargetAreaType, TargetArea> targetAreaDirectory; //Dictionary to easily search up assigned target areas and their values
    protected TargetArea currentHitArea; //Currently shot target area (used to get the respawn wait time addition)

    // Start is called before the first frame update
    void Start()
    {
        currentMode = Mode.Dormant;

        if (navMesh == null)
            navMesh = GetComponent<NavMeshAgent>();

        navMesh.speed = speed;

        targetAreaDirectory = new Dictionary<TargetAreaType, TargetArea>();
        foreach(TargetArea area in targetAreas)
        {
            targetAreaDirectory.Add(area.type, area);
        }

        if (ghostCollider)
            ghostCollider.enabled = true;
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
            case Mode.Exiting:
                Exiting();
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

        targetGridPosition = playerGridPosition;

        Move();

        lastTargetGridPosition = targetGridPosition;

        //Play Sound
        source.loop = true;
        source.clip = chaseSound;

        if(!source.isPlaying || source.clip != chaseSound)
        {
            source.Play();
        }
    }

    //Scatter and move to the provided transforms location
    protected virtual void Scatter()
    {
        Vector2Int scatterTargetGridPos = map.GetGridLocation(scatterTarget.position);

        targetGridPosition = scatterTargetGridPos;

        Move();

        lastTargetGridPosition = targetGridPosition;

        //Play Sound
        if (source.isPlaying)
        {
            source.Stop();
        }
    }

    /// <summary>
    /// Moves the Ghost towards the target location following all the rules of Pac-Man's ghosts
    /// </summary>
    protected void Move()
    {
        navMesh.enabled = true;

        if (currentGridPosition != lastGridPosition || navMesh.remainingDistance < 0.1f)
        {
            float angleToUp = Vector2.Dot(currentDirection, Vector2.up);
            float angleToDown = Vector2.Dot(currentDirection, Vector2.down);
            float angleToRight = Vector2.Dot(currentDirection, Vector2.right);
            float angleToLeft = Vector2.Dot(currentDirection, Vector2.left);

            float distToTargetFromNext = Mathf.Infinity;

            Vector2Int desiredNextGridPosition = currentGridPosition;
            Vector2Int desiredNextDirection = currentDirection;

            if(angleToUp >= -0.1f && map.SampleGrid(currentGridPosition + Vector2Int.up) == Map.GridType.Air)
            {
                Vector2Int nextGridPosUp = currentGridPosition + Vector2Int.up;
                float distanceToUp = Vector2Int.Distance(currentGridPosition, targetGridPosition);
                if(distanceToUp < distToTargetFromNext)
                {
                    distToTargetFromNext = distanceToUp;
                    desiredNextGridPosition = nextGridPosUp;
                    desiredNextDirection = Vector2Int.up;
                }
            }

            if (angleToDown >= -0.1f && map.SampleGrid(currentGridPosition + Vector2Int.down) == Map.GridType.Air)
            {
                Vector2Int nextGridPosDown = currentGridPosition + Vector2Int.down;
                float distanceToDown = Vector2Int.Distance(nextGridPosDown, targetGridPosition);
                if (distanceToDown < distToTargetFromNext)
                {
                    distToTargetFromNext = distanceToDown;
                    desiredNextGridPosition = nextGridPosDown;
                    desiredNextDirection = Vector2Int.down;
                }
            }

            if (angleToLeft >= -0.1f && map.SampleGrid(currentGridPosition + Vector2Int.left) == Map.GridType.Air)
            {
                Vector2Int nextGridPosLeft = currentGridPosition + Vector2Int.left;
                float distanceToLeft = Vector2Int.Distance(nextGridPosLeft, targetGridPosition);
                if (distanceToLeft < distToTargetFromNext)
                {
                    distToTargetFromNext = distanceToLeft;
                    desiredNextGridPosition = nextGridPosLeft;
                    desiredNextDirection = Vector2Int.left;
                }
            }

            if (angleToRight >= -0.1f && map.SampleGrid(currentGridPosition + Vector2Int.right) == Map.GridType.Air)
            {
                Vector2Int nextGridPosRight = currentGridPosition + Vector2Int.right;
                float distanceToRight = Vector2Int.Distance(nextGridPosRight, targetGridPosition);
                if (distanceToRight < distToTargetFromNext)
                {
                    desiredNextGridPosition = nextGridPosRight;
                    desiredNextDirection = Vector2Int.right;
                }
            }

            nextGridPosition = desiredNextGridPosition;
            currentDirection = desiredNextDirection;
        }

        navMesh.SetDestination(map.GetWorldFromGrid(nextGridPosition));
    }

    //Mode at the games start to keep ghost unactive until the required pellets are collected
    protected virtual void Dormant()
    {
        if(Score.pelletsCollected >= pelletsNeededToStart)
            currentMode = Mode.Exiting;
    }

    //Mode for exiting the spawn location
    protected virtual void Exiting()
    {
        Vector2Int spawnExitGridPosition = map.GetGridLocation(spawnExit.position);
        navMesh.SetDestination(map.GetWorldFromGrid(spawnExitGridPosition));

        if(Vector2Int.Distance(spawnExitGridPosition, currentGridPosition) < 0.05f)
        {
            currentMode = Mode.Chase;

            if(exitSpawnToRight)
            {
                currentDirection = Vector2Int.right;
                nextGridPosition = spawnExitGridPosition + Vector2Int.right;
            } else
            {
                currentDirection = Vector2Int.left;
                nextGridPosition = spawnExitGridPosition + Vector2Int.left;
            }
        }
    }

    bool startedRespawnSequence = false; //Used in the Respawn mode to check whether the respawn sequence has started

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

        if (spriteController)
            spriteController.DeactivateColliders();
        if (ghostCollider)
            ghostCollider.enabled = false;

        navMesh.ResetPath();
        startedRespawnSequence = true;

        WaitForSeconds deathWait = new WaitForSeconds(3f);

        yield return deathWait;

        Vector2Int respawnPointGridPos = map.GetGridLocation(respawnPoint.position);
        navMesh.SetDestination(map.GetWorldFromGrid(respawnPointGridPos));

        yield return null;

        WaitUntil arrivedAtRespawnPoint = new WaitUntil(() => navMesh.remainingDistance <= 0.1f);

        yield return new WaitUntil(() => navMesh.remainingDistance <= 0.1f);

        WaitForSeconds respawnWait = new WaitForSeconds(respawnWaitTime + currentHitArea.respawnTimeAddition);

        yield return respawnWait;

        //Reset variables
        startedRespawnSequence = false;

        print("Respawned: " + name);

        spriteRenderer.color = Color.white;

        if(spriteController)
            spriteController.ActivateColliders();
        if(ghostCollider)
            ghostCollider.enabled = true;

        currentMode = Mode.Exiting;
        lastTargetGridPosition = new Vector2Int(-1, -1);
    }

    public virtual void InitiateScatter()
    {
        currentMode = Mode.Scatter;
        currentDirection = -currentDirection;
        nextGridPosition = map.GetNextGridPosition(currentGridPosition, currentDirection, true, true);

        if (ghostCollider)
            ghostCollider.enabled = false;
    }
    public virtual void DeactivateScatter()
    {
        if (currentMode == Mode.Scatter)
        {
            currentMode = Mode.Chase;

            if (ghostCollider)
                ghostCollider.enabled = true;
        }
    }

    //Gets the target area from the directory using the target area type
    public TargetArea GetTargetArea(TargetAreaType type)
    {
        return targetAreaDirectory[type];
    }

    /// <summary>
    /// Called when ghost is hit with the gun and sets the mode to respawn and returns a hit information struct which includes points to add and target area hit benefits
    /// </summary>
    public virtual HitInformation GotHit(TargetAreaType type)
    {
        currentMode = Mode.Respawn;

        HitInformation hit = new HitInformation();
        hit.targetArea = GetTargetArea(type);
        hit.pointWorth = pointWorth;

        currentHitArea = hit.targetArea;

        return hit;
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
