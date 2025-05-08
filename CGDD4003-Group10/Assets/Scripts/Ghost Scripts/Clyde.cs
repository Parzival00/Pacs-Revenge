using UnityEngine;

public class Clyde : Ghost
{
    [Header("Clyde Specific Settings")]
    [SerializeField] int radiusToAvoidPlayer = 4;
    [SerializeField] int randomTurnRange; //1 out of randomTurnRange (Must be > 1)
    [SerializeField] float randomCooldown;
    protected float cooldownTimer;
    protected bool flipped;
    protected override void Chase()
    {
        Vector2Int playerGridPosition = map.GetPlayerPosition();
        Vector2Int clydeGridPosition = map.GetGridLocation(transform.position);

        Vector2Int newTargetPosition;

        if (Vector2Int.Distance(playerGridPosition, clydeGridPosition) < radiusToAvoidPlayer)
        {
            newTargetPosition = map.GetGridLocation(scatterTarget.position);
        }
        else
        {
            newTargetPosition = playerGridPosition;
        }

        targetGridPosition = map.CheckEdgePositions(transform.position, newTargetPosition);


        Move(false);

        lastTargetGridPosition = targetGridPosition;

        PlayChaseSound();

        spriteRenderer.color = Color.white; //Temporary
    }
    /// <summary>
    /// Chase mode from blinky but with a random chance to flip clyde around and force a strange path, making him seem more random and crazy
    /// </summary>
    protected override void Scatter()
    {
        Vector2Int playerGridPosition = map.CheckEdgePositions(transform.position);

        targetGridPosition = playerGridPosition;

        scatterMove(false);

        lastTargetGridPosition = targetGridPosition;
        PlayChaseSound();
        if (cooldownTimer <= 0)
        {
            flipped = false;
        }
        else
        {
            cooldownTimer -= Time.deltaTime;
        }
    }
    /// <summary>
    /// Revision of the move method to include a random chance to force clyde to turn around when close to the player. Only used in scatter mode
    /// </summary>
    protected bool scatterMove(bool canTurnAround)
    {
        if (currentMode != Mode.Chase && currentMode != Mode.Scatter)
            return false;

        navMesh.enabled = true;

        bool turnedAround = false;
        Vector2Int playerGridPosition = map.GetPlayerPosition();
        Vector2Int clydeGridPosition = map.GetGridLocation(transform.position);

        if ((int)Random.Range(0, randomTurnRange) == 1 && Vector2Int.Distance(playerGridPosition, clydeGridPosition) < radiusToAvoidPlayer && !flipped)
        {
            currentDirection = -currentDirection;
            nextGridPosition = map.GetNextGridPosition(currentGridPosition, currentDirection, true, true);
            turnedAround = true;
            flipped = true;
            cooldownTimer = randomCooldown;
            print("Rando");
        }
        else if (currentGridPosition != lastGridPosition || navMesh.remainingDistance < 0.1f)
        {
            float angleToUp = Vector2.Dot(currentDirection, Vector2.up);
            float angleToDown = Vector2.Dot(currentDirection, Vector2.down);
            float angleToRight = Vector2.Dot(currentDirection, Vector2.right);
            float angleToLeft = Vector2.Dot(currentDirection, Vector2.left);

            float distToTargetFromNext = Mathf.Infinity;

            Vector2Int desiredNextGridPosition = currentGridPosition;
            Vector2Int desiredNextDirection = currentDirection;

            if ((canTurnAround || angleToUp >= -0.1f) && map.SampleGrid(currentGridPosition + Vector2Int.up) == Map.GridType.Air)
            {
                Vector2Int nextGridPosUp = currentGridPosition + Vector2Int.up;
                float distanceToUp = Vector2Int.Distance(nextGridPosUp, targetGridPosition);
                if (distanceToUp < distToTargetFromNext)
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
    public override void InitiateScatter()
    {
        if (currentMode == Mode.Chase)
        {
            currentMode = Mode.Scatter;
            if ((int)Random.Range(0, randomTurnRange) == 1)
            {
                currentDirection = -currentDirection;
                nextGridPosition = map.GetNextGridPosition(currentGridPosition, currentDirection, true, true);
                flipped = true;
            }
            else
            {
                flipped = false;
            }
        }
    }
}
