using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clyde : Ghost
{
    [Header("Clyde Specific Settings")]
    [SerializeField] int radiusToAvoidPlayer = 8;
    protected override void Chase()
    {
        Vector2Int playerGridPosition = map.CheckEdgePositions(transform.position);
        Vector2Int clydeGridPosition = map.GetGridLocation(transform.position);

        Vector2Int newTargetPosition;
        
        if (Vector2Int.Distance(playerGridPosition,clydeGridPosition) < radiusToAvoidPlayer)
        {
            newTargetPosition = map.GetGridLocation(scatterTarget.position);
        }
        else
        {
            newTargetPosition = playerGridPosition;
        }

        targetGridPosition = map.CheckEdgePositions(transform.position, newTargetPosition);
        

        Move();

        lastTargetGridPosition = targetGridPosition;

        PlayChaseSound();
    }
}
