using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inky : Ghost
{
    [Header("Inky Specific Settings")]
    [SerializeField] Transform blinkyTransform;

    //Temporary Movement to player
    protected override void Chase()
    {
        /*System.Random distanceForward = new System.Random();
        Vector2Int rand = new Vector2Int(distanceForward.Next(5),distanceForward.Next(5));*/

        Vector2Int playerGridPosition = map.GetGridLocation(player.position);
        Vector2Int playerTwoAheadGridPos = map.GetGridPositionAhead(playerGridPosition, map.GetGridSpaceDirection(player.forward), 2, true, true);
        Vector2Int blinkyGridPosition = map.GetGridLocation(blinkyTransform.position);

        Vector2Int fromBlinkyToPlayerAhead = playerTwoAheadGridPos - blinkyGridPosition;

        Vector2Int newTargetGridPosition = map.GetGridPositionAhead(blinkyGridPosition, fromBlinkyToPlayerAhead * 2, true, true);

        targetGridPosition = map.CheckEdgePositions(transform.position, newTargetGridPosition);// + rand;

        Move();

        lastTargetGridPosition = targetGridPosition;

        PlayChaseSound();
    }

    ///Summary
    ///Take Blinky distance to player || do rand in between those position to set inky
    ///Summary
}
