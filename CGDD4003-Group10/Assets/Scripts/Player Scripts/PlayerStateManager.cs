using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    public bool CanFire { get; private set; } = true;
    public bool CanBeDamaged { get; private set; } = true;
    public bool IsTrapped { get; private set; } = false;
    public bool IsDead { get; private set; } = false;
    public bool GunActivated { get; private set; } = true;

    public event Action OnDeath;
    public event Action OnRespawn;
    public event Action<int> OnLevelFinish;
    public event Action<bool> OnMoveStateChanged;
    public event Action<bool> OnTrappedStateChanged;
    public event Action<bool> OnDamagableStateChanged;
    public event Action<bool> OnFireStateChanged;
    public event Action<bool> OnGunStateChanged;

    public void SetMoveState(bool canMove)
    {
        if (CanMove != canMove)
        {
            CanMove = canMove;
            OnMoveStateChanged?.Invoke(canMove);
        }
    }

    public void SetDamagableState(bool canBeDamaged)
    {
        if (CanBeDamaged != canBeDamaged)
        {
            CanBeDamaged = canBeDamaged;
            OnDamagableStateChanged?.Invoke(canBeDamaged);
        }
    }

    public void SetFireState(bool canFire)
    {
        if (CanFire != canFire)
        {
            CanFire = canFire;
            OnFireStateChanged?.Invoke(canFire);
        }
    }

    public void SetGunActivated(bool gunActivated)
    {
        if(GunActivated != gunActivated)
        {
            GunActivated = gunActivated;
            OnGunStateChanged?.Invoke(gunActivated);
        }
    }

    public void SetTrappedState(bool isTrapped)
    {
        if(IsTrapped != isTrapped)
        {
            IsTrapped = isTrapped;
            OnTrappedStateChanged?.Invoke(isTrapped);
        }
    }

    public void Die()
    {
        IsDead = true;
        SetMoveState(false);
        SetFireState(false);
        SetTrappedState(false);
        SetDamagableState(false);
        OnDeath?.Invoke();
    }

    public void Respawn()
    {
        OnRespawn?.Invoke();
        IsDead = false;
    }

    public void LevelFinish(int level)
    {
        OnLevelFinish?.Invoke(level);
    }
}
