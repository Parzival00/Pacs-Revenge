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

    [Header("Ghost Settings")]
    [SerializeField] protected int pelletsNeededToStart = 10;
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected float respawnWaitTime = 5f;

    [Header("Transform Targets")]
    [SerializeField] protected Transform player;
    [SerializeField] protected Transform scatterTarget;
    [SerializeField] protected Transform respawnPoint;

    protected Vector3 targetPosition;
    protected Vector2Int lastPlayerGridPosition;

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
        Act();
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
        Vector2Int playerGridPosition = map.GetPlayerPosition();

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
        currentMode = Mode.Chase;
    }

    bool startedRespawnSequence = false; //Used in the Respawn mode to check whether the timer has started
    float respawnTimer; //Used in the Respawn mode to hold the time when the ghost will change back to Chase mode

    //Mode activated when a ghost is shot. Ghost moves to provided respawn location and wants specified amount of time before shifting to Chase mode
    protected virtual void Respawn()
    {
        Vector2Int respawnPointGridPos = map.GetGridLocation(respawnPoint.position);

        navMesh.SetDestination(map.GetWorldFromGrid(respawnPointGridPos));

        if (navMesh.remainingDistance <= 0.1f)
        {
            if (startedRespawnSequence == false)
            {
                startedRespawnSequence = true;
                respawnTimer = Time.time + respawnWaitTime;
            }
        }

        if(startedRespawnSequence && Time.time >= respawnTimer)
        {
            startedRespawnSequence = false;
            currentMode = Mode.Chase;

            lastPlayerGridPosition = new Vector2Int(-1, -1);
        }
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
}
