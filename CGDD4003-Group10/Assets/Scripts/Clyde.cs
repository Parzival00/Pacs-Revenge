using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clyde : Ghost
{
    [SerializeField] int radiusToAvoidPlayer;
    protected override void Chase()
    {
        Vector2Int playerGridPosition = map.CheckEdgePositions(transform.position);
        Vector2Int clydeGridPosition = map.GetGridLocation(transform.position);
        
        if (Vector2Int.Distance(playerGridPosition,clydeGridPosition) < 8)
        {
            print("within clyde radius");
            targetGridPosition = map.GetGridLocation(scatterTarget.position);
        }
        else
        {
            targetGridPosition = playerGridPosition;
        }
        

        Move();

        lastTargetGridPosition = targetGridPosition;
    }
}
