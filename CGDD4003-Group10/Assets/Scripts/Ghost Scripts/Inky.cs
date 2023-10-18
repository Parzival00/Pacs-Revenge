using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inky : Ghost
{
    [Header("Inky Specific Settings")]
    [SerializeField] Transform blinkyTransform;
    [SerializeField] float targetRayMultiplier = 1.5f;
    [SerializeField] protected float dashSpeedMultiplier;
    [SerializeField] protected float dashTime;
    [SerializeField] protected float dashCooldown;
    protected float cooldownTimer;
    protected float dashTimer;
    protected bool canDash;
    protected bool dashing;
    protected float baseSpeed;

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
    protected override void Scatter()
    {
        if (dashing && dashTimer <= 0)
        {
            //ends the dash
            dashing = false;
            cooldownTimer = dashCooldown;
            speed = baseSpeed;
        }
        else if (canDash && !dashing && cooldownTimer <= 0) 
        {
            //starts the dash
            print("Dashing");
            speed *= dashSpeedMultiplier;
            dashing = true;
            canDash = false;
            dashTimer = dashTime;
        }
        else if (dashing && dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
        }

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
        else
        {
            canDash = true;
            cooldownTimer = 0;
        }

        Chase();
    }
    public override void InitiateScatter()
    {
        if (currentMode == Mode.Chase)
        {
            baseSpeed = speed;
            currentMode = Mode.Scatter;
            dashTimer = 0;
            cooldownTimer = dashCooldown;
        }
    }
    public override void DeactivateScatter()
    {
        if (currentMode == Mode.Scatter)
        {
            speed = baseSpeed;
            canDash = false;
            dashing = false;
            currentMode = Mode.Chase;
        }
    }
    ///Summary
    ///Take Blinky distance to player || do rand in between those position to set inky
    ///Summary
}
