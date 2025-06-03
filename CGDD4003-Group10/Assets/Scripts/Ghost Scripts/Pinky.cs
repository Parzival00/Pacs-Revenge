using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinky : Ghost
{
    [Header("Pinky Specific Settings")]
    [SerializeField] int spacesAheadOfPlayer = 2;
    [SerializeField] float radiusToAvoidPlayer;
    [SerializeField] float turnCooldown;
    [Tooltip("The distance for which Pinky will target the player instead of ahead of the player")]
    [SerializeField] float aggroDistThreshold = 2f;
    protected float cooldownTimer;
    protected bool justFlippedDirection;

    [Tooltip("Pinky overflow error: (Pinky had a bug in the original game causing an error choosing a target location in specific situations)")]
    [SerializeField] bool originalMode; 
    /// <summary>
    /// Overrides chase mode to choose the space 4 tiles ahead of pac-man instead of pac-man as the target
    /// In the original game there was a bug where if pac-man was facing up pinky would target the space 4 ahead AND 4 to the left of pac-man instead
    /// The bool originalMode can be turned off, fixing the bug
    /// </summary>
    protected override void Chase()
    {
        Vector2Int playerGridPosition = map.GetPlayerPosition();
        Vector2Int playerGridDir = map.GetGridSpaceDirection(player.forward);
        Vector2Int pinkyGridTarget = map.GetGridPositionAhead(playerGridPosition, playerGridDir, spacesAheadOfPlayer, true, false);

        if(Vector2Int.Distance(playerGridPosition, currentGridPosition) < aggroDistThreshold)
        {
            pinkyGridTarget = playerGridPosition;
        }

        if (originalMode)
        {
            if (playerGridDir == Vector2Int.up)
            {
                Vector2Int tempTarget = map.GetGridPositionAhead(playerGridPosition, playerGridDir, 4, true, false);
                pinkyGridTarget = map.GetGridPositionAhead(tempTarget, Vector2Int.left, 4, true, false);
            }
        }
        
        targetGridPosition = map.CheckEdgePositions(this.transform.position, pinkyGridTarget);

        Move(false);

        lastTargetGridPosition = targetGridPosition;

        PlayChaseSound();

        //spriteRenderer.color = Color.white; //Temporary
    }

    protected override void Scatter()
    {
        ScatterTimer();

        Vector2Int playerGridPosition = map.GetPlayerPosition();
        Vector2Int pinkyGridPosition = map.GetGridLocation(transform.position);
        Vector2Int playerGridDir = map.GetGridSpaceDirection(player.forward);
        

        if (Vector2Int.Distance(playerGridPosition, pinkyGridPosition) < radiusToAvoidPlayer && -currentDirection == playerGridDir && !justFlippedDirection)
        {
            currentDirection = -currentDirection;
            nextGridPosition = map.GetNextGridPosition(currentGridPosition, currentDirection, true, true);
            justFlippedDirection = true;
            cooldownTimer = turnCooldown;
            targetGridPosition = nextGridPosition;
            print("Pinky Flipped Direction");
        }
        else
        {
            Vector2Int pinkyGridTarget = map.GetGridPositionAhead(playerGridPosition, -playerGridDir, spacesAheadOfPlayer + 2, true, false);
            //Vector2Int scatterTargetGridPos = map.GetGridLocation(scatterTarget.position);
            targetGridPosition = map.CheckEdgePositions(transform.position, pinkyGridTarget);
        }
        Move(false);
        lastTargetGridPosition = targetGridPosition;
    }
    protected void ScatterTimer()
    {
        if(cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
        else
        {
            justFlippedDirection = false;
        }
    }

}
