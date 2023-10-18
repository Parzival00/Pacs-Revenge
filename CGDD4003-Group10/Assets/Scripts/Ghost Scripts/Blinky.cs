using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinky : Ghost
{
    [Header("Blinky specific settings")]
    [SerializeField] float cooldownTime;
    protected float cooldownTimer;
    protected bool canTurn;
    //Blinky's chase is the simplest being go to the player location so just uses base functionality of Chase()
    protected override void Chase()
    {
        base.Chase();
    }

    protected override void Scatter()
    {
        Vector2Int playerGridPosition = map.CheckEdgePositions(transform.position);

        targetGridPosition = playerGridPosition;

        if(canTurn)
        {
            canTurn = !Move(true);
            //This happens if blinky turned in the last move call
            if(!canTurn)
            {
                cooldownTimer = cooldownTime;
                print("turned");
            }
        }
        else 
        {
            Move(false);
            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;    
            }
            else
            {
                canTurn = true;
            }
        }
        lastTargetGridPosition = targetGridPosition;
        PlayChaseSound();
    }
    public override void InitiateScatter()
    {
        if (currentMode == Mode.Chase)
        {
            currentMode = Mode.Scatter;
            canTurn = false;
            cooldownTimer = cooldownTime;
        }
    }
    public override void DeactivateScatter()
    {
        if (currentMode == Mode.Scatter)
        {
            currentMode = Mode.Chase;
            canTurn = false;
        }
    }

}
