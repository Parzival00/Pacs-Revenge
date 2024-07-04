using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Ghost
{
    public float attackCooldown;
    private float attackTimer = 0f;

    private void Update()
    {
        attackTimer += Time.deltaTime;
    }

    //For now the boss will just follow the same chasing pattern as blinky
    protected override void Chase()
    {
        Vector2Int playerGridPosition = map.CheckEdgePositions(transform.position);

        targetGridPosition = playerGridPosition;

        Move(false);

        lastTargetGridPosition = targetGridPosition;
        PlayChaseSound();

        if(attackTimer >= attackCooldown)
        {
            Attack();
        }
    }

    protected override void Bossfight()
    {
        base.Bossfight();
    }

    //We dont want the boss to scatter, so it just stays in chase
    protected override void Scatter()
    {
        currentMode = Mode.Chase;
    }

    //We dont want the boss to scatter, so it just stays in chase
    public override void InitiateScatter()
    {
        currentMode = Mode.Chase;
    }
    public override void DeactivateScatter()
    {
        if (currentMode == Mode.Scatter)
        {
            currentMode = Mode.Chase;
        }
    }

    //For now it just does a simple random generator to select the next attack
    //In the future I will make it so that it can potentially be weighted and varied based on which heads are active or not
    private void Attack()
    {
        switch (Random.RandomRange(0, 4)) 
        {
            case 0:
                ClydeAttack();
                break;
            case 1:
                BlinkyAttack();
                break;
            case 2:
                PinkyAttack();
                break;
            case 3:
                InkyAttack();
                break;
        }

    }

    private void PinkyAttack()
    {

    }

    private void BlinkyAttack()
    {

    }

    private void InkyAttack()
    {

    }

    private void ClydeAttack()
    {

    }
}
