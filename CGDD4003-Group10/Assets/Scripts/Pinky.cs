using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinky : Ghost
{
    [Header("Pinky overflow error: (Pinky had a bug in the original game causing an error choosing a target location in specific situations)")]
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
        Vector2Int pinkyGridTarget = map.GetGridPositionAhead(playerGridPosition, playerGridDir, 4, true, false);

        if (originalMode)
        {
            if (playerGridDir == Vector2Int.up)
            {
                Vector2Int tempTarget = map.GetGridPositionAhead(playerGridPosition, playerGridDir, 4, true, false);
                pinkyGridTarget = map.GetGridPositionAhead(tempTarget, Vector2Int.left, 4, true, false);
            }
        }
        
        targetGridPosition = map.CheckEdgePositions(this.transform.position, pinkyGridTarget);

        Move();

        lastTargetGridPosition = targetGridPosition;

        //Play Sound
        source.loop = true;
        source.clip = chaseSound;

        if (!source.isPlaying || source.clip != chaseSound)
        {
            source.Play();
        }
    }

    
}
