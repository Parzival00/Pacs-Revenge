using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public bool gunActivated { get; private set; }
    protected float gunTimer;

    protected PlayerController playerController;

    public void ActivateGun(PlayerController playerController, float gunTimeAmount)
    {
        this.playerController = playerController;
        gunTimerCoroutine = StartCoroutine(GunTimer(gunTimeAmount));

        gunActivated = true;
    }
    Coroutine gunTimerCoroutine;
    protected IEnumerator GunTimer(float gunTimeAmount)
    {
        gunTimer = gunTimeAmount;

        while (gunTimer >= 0)
        {
            gunTimer -= Time.deltaTime;
            yield return null;
        }

        StartCoroutine(playerController.DeactivateGun());
        gunTimer = gunTimeAmount;
    }
    public void DeactivateGun()
    {
        gunActivated = false;
    }

    /// <summary>
    /// This event is called whenever the mouse button is initially pressed down
    /// </summary>
    public abstract void OnMouseDownEvent();
    /// <summary>
    /// This event is called for every frame the mouse button is pressed down
    /// </summary>
    public abstract void OnMouseEvent();
    /// <summary>
    /// This event is called whenever the mouse button is lift up
    /// </summary>
    public abstract void OnMouseUpEvent();
    /// <summary>
    /// This event is called every frame
    /// </summary>
    public abstract void OnPassiveEvent();
}
