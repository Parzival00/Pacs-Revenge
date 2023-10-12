using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inky : Ghost
{
    [Header("Inky Specific Settings")]
    [SerializeField] Transform blinkyTransform;
    [SerializeField] float targetRayMultiplier = 1.5f;

    //Temporary Movement to player
    protected override void Chase()
    {
        /*System.Random distanceForward = new System.Random();
        Vector2Int rand = new Vector2Int(distanceForward.Next(5),distanceForward.Next(5));*/

        Vector2Int playerGridPosition = map.GetGridLocation(player.position);
        Vector2Int playerTwoAheadGridPos = map.GetGridPositionAhead(playerGridPosition, map.GetGridSpaceDirection(player.forward), 2, true, true);
        Vector2Int blinkyGridPosition = map.GetGridLocation(blinkyTransform.position);

        Vector2Int fromBlinkyToPlayerAhead = playerTwoAheadGridPos - blinkyGridPosition;

        Vector2Int newRayAhead = new Vector2Int(Mathf.RoundToInt(fromBlinkyToPlayerAhead.x * targetRayMultiplier), Mathf.RoundToInt(fromBlinkyToPlayerAhead.y * targetRayMultiplier));

        Vector2Int newTargetGridPosition = map.GetGridPositionAhead(blinkyGridPosition, newRayAhead, true, true);

        targetGridPosition = map.CheckEdgePositions(transform.position, newTargetGridPosition);// + rand;

        Move(false);

        lastTargetGridPosition = targetGridPosition;

        PlayChaseSound();

        spriteRenderer.color = Color.white; //Temporary
    }

    ///Summary
    ///Take Blinky distance to player || do rand in between those position to set inky
    ///Summary
}
