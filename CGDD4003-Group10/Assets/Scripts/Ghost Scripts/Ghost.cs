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
        public TargetAreaDifficulty difficulty;
        public float healthValue;
    }

    public struct HitInformation
    {
        public TargetArea targetArea;
        public int pointWorth;
        public GameObject bigBlood;
        public GameObject smallBlood;
    }

    public enum Mode
    {
        Dormant, //Start mode when ghosts hasnt left spawn
        Exiting, //Ghost is exiting spawn
        Chase, //Ghost is chasing player
        Scatter, //Ghost is scattering because player has the railgun
        Respawn, //Ghost is respawning
        Freeze, //Ghost is stunned
        Flinch, //Ghost got hit by the railgun but didnt die
        Reseting //Ghost is getting reset because player got killed
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

    public enum TargetAreaDifficulty
    {
        Easy,
        Medium,
        Hard
    }

    protected Mode currentMode;
    protected Mode previousMode;
    public Mode CurrentMode { get => currentMode; }

    [Header("Debug Settings")]
    [SerializeField] bool visualizeTargetPosition;

    [Header("References")]
    [SerializeField] protected NavMeshAgent navMesh;
    [SerializeField] protected Map map;
    [SerializeField] protected Transform ghostIcon;
    [SerializeField] protected GhostSpriteController spriteController;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected GameObject bigBlood;
    [SerializeField] protected GameObject smallBlood;
    [SerializeField] protected GameObject corpse;
    [SerializeField] protected GameObject stunEffect;
    [SerializeField] protected GameObject minimapIcon;
    [SerializeField] protected GameObject deadMinimapIcon;

    [Header("Ghost Settings")]
    [SerializeField] protected bool exitSpawnToRight;
    [SerializeField] protected int pelletsNeededToStart = 10;
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected float respawnWaitTime = 5f;
    [SerializeField] protected float freezeTime;
    protected float freezeTimer;
    [SerializeField] protected int pointWorth = 20;
    [SerializeField] protected TargetArea[] targetAreas;
    [SerializeField] protected float ghostHealth;
    [SerializeField] protected float flinchLength = 0.5f;
    [SerializeField] protected bool faceForwardForDeath = true;

    [Header("Transform Targets")]
    [SerializeField] protected Transform player;
    [SerializeField] protected Transform scatterTarget;
    [SerializeField] protected Transform respawnPoint;
    [SerializeField] protected Transform spawnExit;

    [Header("Player Dectecting Collider")]
    [SerializeField] Collider ghostCollider;

    [Header("Chase Sound Settings")]
    [SerializeField] protected AudioSource chaseSoundSource;
    [SerializeField] protected float farPitch = 0.15f;
    //[SerializeField] protected float farVolume  = 0.15f;
    [SerializeField] protected AudioClip chaseSoundFar;
    [SerializeField] protected float closeSoundBlendpadding = 1f;
    [SerializeField] protected float dstToBlendToCloseSound = 10;
    [SerializeField] protected float closePitch = 3f;
    //[SerializeField] protected float closeVolume = 3f;
    //[SerializeField] protected AudioClip chaseSoundClose;
    //[SerializeField] protected AudioClip scatterSound;
    //[SerializeField] protected AudioClip deathSound;
    //[SerializeField] protected AudioClip corpseMoveSound;
    //[SerializeField] protected AudioClip respawnedSound;

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

        if (map == null)
            map = GameObject.FindObjectOfType<Map>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (spriteController == null)
            spriteController = GetComponentInChildren<GhostSpriteController>();

        navMesh.speed = speed;

        stunEffect.SetActive(false);

        targetAreaDirectory = new Dictionary<TargetAreaType, TargetArea>();
        foreach(TargetArea area in targetAreas)
        {
            targetAreaDirectory.Add(area.type, area);
        }

        //if (ghostCollider)
         //   ghostCollider.enabled = true;
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
            case Mode.Freeze:
                Freeze();
                break;
            case Mode.Flinch:
                Flinch();
                break;
            default:
                break;
        }
    }

    //Chase the player
    protected virtual void Chase()
    {
        //spriteRenderer.color = Color.white; //Temporary

        Vector2Int playerGridPosition = map.CheckEdgePositions(transform.position);

        targetGridPosition = playerGridPosition;

        Move(false);

        lastTargetGridPosition = targetGridPosition;
        PlayChaseSound();
    }

    protected void PlayChaseSound()
    {
        Vector2Int playerGridPosition = map.CheckEdgePositions(transform.position);

        float distToPlayer = Vector2Int.Distance(playerGridPosition, currentGridPosition);

        //Play Sound
        chaseSoundSource.loop = true;
        chaseSoundSource.clip = chaseSoundFar;


        if (distToPlayer < dstToBlendToCloseSound)
        {
            chaseSoundSource.pitch = closePitch;
            //chaseSoundSource.volume = closeVolume;
            //chaseSoundSource.clip = chaseSoundClose;
        }
        else if (distToPlayer > dstToBlendToCloseSound + closeSoundBlendpadding)
        {
            chaseSoundSource.pitch = farPitch;
            //chaseSoundSource.volume = farVolume;
            //chaseSoundSource.clip = chaseSoundFar;
        }

        if (!chaseSoundSource.isPlaying || (chaseSoundSource.clip != chaseSoundFar /*&& chaseSoundSource.clip != chaseSoundClose*/))
        {
            /*if(chaseSoundSource.clip == chaseSoundFar)
                chaseSoundSource.pitch = farPitch;

            if (chaseSoundSource.clip == chaseSoundClose)
                chaseSoundSource.pitch = closePitch;*/


            chaseSoundSource.Play();
        }
    }

    //Scatter and move to the provided transforms location
    protected virtual void Scatter()
    {
        //spriteRenderer.color = Color.white; //Temporary

        Vector2Int scatterTargetGridPos = map.GetGridLocation(scatterTarget.position);

        targetGridPosition = scatterTargetGridPos;

        Move(false);

        lastTargetGridPosition = targetGridPosition;

        //Play Sound
        if (chaseSoundSource.isPlaying)
        {
            //chaseSoundSource.pitch = 1.5f;
        }
    }

    /// <summary>
    /// Moves the Ghost towards the target location following all the rules of Pac-Man's ghosts. Takes in whether or not the ghost can turn around and returns whether the ghost did turn around
    /// </summary>
    protected bool Move(bool canTurnAround)
    {
        if (currentMode != Mode.Chase && currentMode != Mode.Scatter)
            return false;

        navMesh.enabled = true;

        bool turnedAround = false;

        if (currentGridPosition != lastGridPosition || navMesh.remainingDistance < 0.1f)
        {
            float angleToUp = Vector2.Dot(currentDirection, Vector2.up);
            float angleToDown = Vector2.Dot(currentDirection, Vector2.down);
            float angleToRight = Vector2.Dot(currentDirection, Vector2.right);
            float angleToLeft = Vector2.Dot(currentDirection, Vector2.left);

            float distToTargetFromNext = Mathf.Infinity;

            Vector2Int desiredNextGridPosition = currentGridPosition;
            Vector2Int desiredNextDirection = currentDirection;

            if((canTurnAround || angleToUp >= -0.1f) && map.SampleGrid(currentGridPosition + Vector2Int.up) == Map.GridType.Air)
            {
                Vector2Int nextGridPosUp = currentGridPosition + Vector2Int.up;
                float distanceToUp = Vector2Int.Distance(nextGridPosUp, targetGridPosition);
                if(distanceToUp < distToTargetFromNext)
                {
                    distToTargetFromNext = distanceToUp;
                    desiredNextGridPosition = nextGridPosUp;
                    desiredNextDirection = Vector2Int.up;
                }
            }

            if ((canTurnAround || angleToDown >= -0.1f) && map.SampleGrid(currentGridPosition + Vector2Int.down) == Map.GridType.Air)
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

            if ((canTurnAround || angleToLeft >= -0.1f) && map.SampleGrid(currentGridPosition + Vector2Int.left) == Map.GridType.Air)
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

            if ((canTurnAround || angleToRight >= -0.1f) && map.SampleGrid(currentGridPosition + Vector2Int.right) == Map.GridType.Air)
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
            turnedAround = Vector2.Dot(currentDirection, desiredNextDirection) < -0.1f;
            currentDirection = desiredNextDirection;
        }

        navMesh.SetDestination(map.GetWorldFromGrid(nextGridPosition));

        return turnedAround;
    }

    //Mode at the games start to keep ghost unactive until the required pellets are collected
    protected virtual void Dormant()
    {
        spriteController.DeactivateColliders();

        if(Score.pelletsCollected >= pelletsNeededToStart)
            currentMode = Mode.Exiting;
    }

    bool isFlinching = false;
    float flinchTimer;
    protected virtual void Flinch()
    {
        navMesh.enabled = false;

        if(!isFlinching)
        {
            isFlinching = true;
            flinchTimer = Time.time + flinchLength;
        }

        if(isFlinching && Time.time >= flinchTimer)
        {
            isFlinching = false;
            currentMode = previousMode;
        }
    }

    #region Exiting Spawn
    //Mode for exiting the spawn location
    protected virtual void Exiting()
    {
        if(!startedExiting)
            exitingCoroutine = StartCoroutine(ExitingSequence());
    }

    Coroutine exitingCoroutine;
    bool startedExiting;
    protected virtual IEnumerator ExitingSequence()
    {
        startedExiting = true;

        Vector2Int spawnExitGridPosition = map.GetGridLocation(spawnExit.position);
        navMesh.SetDestination(map.GetWorldFromGrid(spawnExitGridPosition));

        yield return null;

        WaitUntil arrivedAtExitPoint = new WaitUntil(() => currentMode != Mode.Exiting || Vector2Int.Distance(spawnExitGridPosition, currentGridPosition) <= 0.1f);

        yield return arrivedAtExitPoint;

        print("Exited");

        if (PlayerController.gunActivated)
        {
            currentMode = Mode.Scatter;
            currentDirection = -currentDirection;
            nextGridPosition = map.GetNextGridPosition(currentGridPosition, currentDirection, true, true);
        }
        else
        {
            currentMode = Mode.Chase;

            if (exitSpawnToRight)
            {
                currentDirection = Vector2Int.right;
                nextGridPosition = spawnExitGridPosition + Vector2Int.right;
            }
            else
            {
                currentDirection = Vector2Int.left;
                nextGridPosition = spawnExitGridPosition + Vector2Int.left;
            }
        }

        startedExiting = false;

        spriteController.ActivateColliders();
    }

    #endregion

    #region Respawn
    bool startedRespawnSequence = false; //Used in the Respawn mode to check whether the respawn sequence has started

    //Mode activated when a ghost is shot. Ghost moves to provided respawn location and wants specified amount of time before shifting to Chase mode
    protected virtual void Respawn()
    {
        if (!startedRespawnSequence)
        {
            respawnCoroutine = StartCoroutine(RespawnSequence());
        }
    }

    Coroutine respawnCoroutine;
    protected virtual IEnumerator RespawnSequence()
    {
        //spriteRenderer.color = Color.black;

        chaseSoundSource.Stop();

        spriteController.DeactivateColliders();

        navMesh.enabled = false;

        stunEffect.SetActive(false);

        startedRespawnSequence = true;

        spriteController.StartDeathAnimation(faceForwardForDeath);

        minimapIcon.SetActive(false);
        deadMinimapIcon.SetActive(true);
        

        WaitForSeconds deathWait = new WaitForSeconds(3f);

        yield return deathWait;

        if (corpse != null)
        {
            Instantiate(corpse, transform.position, transform.rotation);
        }

        spriteController.StartMovingCorpse();

        yield return new WaitForSeconds(0.5f);

        navMesh.enabled = true;

        Vector2Int respawnPointGridPos = map.GetGridLocation(respawnPoint.position);
        navMesh.SetDestination(map.GetWorldFromGrid(respawnPointGridPos));

        yield return null;

        WaitUntil arrivedAtRespawnPoint = new WaitUntil(() => currentMode != Mode.Respawn || Vector2Int.Distance(respawnPointGridPos, currentGridPosition) <= 0.1f);

        yield return arrivedAtRespawnPoint;
        print("Arrived at Respawn Point");
        

        spriteController.StartReformAnimation();

        WaitForSeconds respawnWait = new WaitForSeconds(respawnWaitTime + currentHitArea.respawnTimeAddition);

        yield return respawnWait;

        spriteController.EndRespawning();

        //Reset variables
        startedRespawnSequence = false;

        print("Respawned: " + name);
        minimapIcon.SetActive(true);
        deadMinimapIcon.SetActive(false);

        //spriteRenderer.color = Color.white;

        spriteController.ActivateColliders();
        //if(ghostCollider)
        //    ghostCollider.enabled = true;

        if(currentMode == Mode.Respawn)
            currentMode = Mode.Exiting;
        lastTargetGridPosition = new Vector2Int(-1, -1);
    }

    #endregion

    public virtual void InitiateScatter()
    {
        if (currentMode == Mode.Chase)
        {
            currentMode = Mode.Scatter;
            currentDirection = -currentDirection;
            nextGridPosition = map.GetNextGridPosition(currentGridPosition, currentDirection, true, true);
        }
    }
    public virtual void DeactivateScatter()
    {
        if (currentMode == Mode.Scatter)
        {
            currentMode = Mode.Chase;

            //if (ghostCollider)
            //    ghostCollider.enabled = true;
        }
    }


    #region Reseting Ghosts
    public void StopGhost()
    {
        currentMode = Mode.Reseting;
        navMesh.enabled = false;
        chaseSoundSource.Stop();
    }

    /// <summary>
    /// resets the ghost back to its spawn point. Used when player dies
    /// delete this if it can be used with SetPosition() instead, Im not sure about turning off the navmesh
    /// -Ben
    /// </summary>
    public void ResetGhost()
    {
        if (resetCoroutine != null)
            StopCoroutine(resetCoroutine);

        if (startedRespawnSequence && respawnCoroutine != null)
        {
            startedRespawnSequence = false;
            StopCoroutine(respawnCoroutine);
        }

        if (startedExiting && exitingCoroutine != null)
        {
            startedExiting = false;
            StopCoroutine(exitingCoroutine);
        }

        isFlinching = false; //Accounts for the rare case of the ghost flinching right when the player dies

        resetCoroutine = StartCoroutine(ResetSequence());
    }

    Coroutine resetCoroutine;
    IEnumerator ResetSequence()
    {
        SetPosition(respawnPoint.position);

        WaitForSeconds wait = new WaitForSeconds(5f);

        yield return wait;

        navMesh.enabled = true;

        stunEffect.SetActive(false);

        spriteController.ResetParameters();

        currentMode = Mode.Dormant;
    }
    #endregion

    #region Freeze/Stun Ghosts
    public void FreezeGhost()
    {
        if (currentMode == Mode.Scatter || currentMode == Mode.Chase)
        {
            freezeTimer = freezeTime;
            previousMode = currentMode;
            currentMode = Mode.Freeze;
            navMesh.enabled = false;
            stunEffect.SetActive(true);
            chaseSoundSource.Stop();
        }
    }

    /// <summary>
    /// Takes the previous mode of the ghost and switches it from freeze back to the previous mode
    /// </summary>
    /// <param name="p">Previous mode</param>
    public void UnFreezeGhost()
    {
        if (currentMode == Mode.Freeze)
        {
            navMesh.enabled = true;
            chaseSoundSource.Play();
            stunEffect.SetActive(false);
            currentMode = previousMode;
        }
        freezeTimer = freezeTime;
    }

    void Freeze()
    {
        if (FreezeCoroutine != null)
        {
            StopCoroutine(FreezeCoroutine);
        }
        FreezeCoroutine = StartCoroutine(FreezeTimer());
    }

    Coroutine FreezeCoroutine;
    IEnumerator FreezeTimer()
    {
        while (freezeTimer >= 0)
        {
            freezeTimer -= Time.deltaTime;
            yield return null;
        }
        UnFreezeGhost();
    }
    public void OnTriggerEnter(Collider c)
    {
        if(c.gameObject.tag == "Stun")
        {
            Destroy(c.gameObject);
            FreezeGhost();
        }
    }
    #endregion


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
        HitInformation hit = new HitInformation();
        hit.targetArea = GetTargetArea(type);
        hit.pointWorth = pointWorth;
        hit.bigBlood = bigBlood;
        hit.smallBlood = smallBlood;

        currentHitArea = hit.targetArea;

        //damage the ghost
        ghostHealth -= currentHitArea.healthValue;
        print("subtracted: " + currentHitArea.healthValue + " health: " + ghostHealth);

        if (ghostHealth <= 0)
        {
            print("respawning");
            ghostHealth = 100;
            currentMode = Mode.Respawn;
        }
        else
        {
            previousMode = currentMode;
            currentMode = Mode.Flinch;
        }

        return hit;
    }

    public virtual TargetAreaDifficulty GetDifficulty(TargetAreaType targetAreaType)
    {
        if(targetAreaDirectory.ContainsKey(targetAreaType))
        {
            return targetAreaDirectory[targetAreaType].difficulty;
        } else
        {
            return TargetAreaDifficulty.Easy;
        }
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

    //Debug Function
    private void OnDrawGizmos()
    {
        if (visualizeTargetPosition)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(map.GetWorldFromGrid(targetGridPosition), Vector3.one);

            Gizmos.color = Color.white;
        }
    }
}
