using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clyde : Ghost
{
    [Header("Clyde Specific Settings")]
    [SerializeField] int radiusToAvoidPlayer = 4;
    protected override void Chase()
    {
        Vector2Int playerGridPosition = map.GetPlayerPosition();
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
        

        Move(false);

        lastTargetGridPosition = targetGridPosition;

        PlayChaseSound();

        spriteRenderer.color = Color.white; //Temporary
    }
}
