using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected WeaponInfo weaponInfo;
    [SerializeField] protected Animator gunAnimator;
    public Animator GunAnimator { get => gunAnimator; }

    [Header("Invisibility Power-up")]
    [SerializeField] protected Material gunMaterial;

    [Header("Weapon Audio")]
    [SerializeField] protected AudioClip gunshotSFX;
    [SerializeField] protected AudioSource weaponSound;

    public bool gunActivated { get; private set; }
    protected float gunTimer;
    protected float gunTimeAmount;

    protected PlayerController playerController;

    public void ActivateGun(PlayerController playerController, float gunTimeAmount)
    {
        this.playerController = playerController;
        //gunTimerCoroutine = StartCoroutine(GunTimer(gunTimeAmount));
        ResetWeapon();

        //gunActivated = true;
    }
    Coroutine gunTimerCoroutine;
    protected IEnumerator GunTimer(float gunTimeAmount)
    {
        this.gunTimeAmount = gunTimeAmount;
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

    public abstract void ResetWeapon();

    /// <summary>
    /// This event is called whenever the mouse button is initially pressed down
    /// </summary>
    public abstract void OnMouseDownEvent();
    /// <summary>
    /// This event is called for every frame the mouse button is pressed down
    /// </summary>
    public abstract void OnMouseEvent();
    /// <summary>
    /// This event is called for every frame the mouse button is not pressed down
    /// </summary>
    public abstract void OnNoMouseEvent();
    /// <summary>
    /// This event is called whenever the mouse button is lift up
    /// </summary>
    public abstract void OnMouseUpEvent();
    /// <summary>
    /// This event is called every frame
    /// </summary>
    public abstract void OnPassiveEvent();

    /// <summary>
    /// This event is called while the timer is active
    /// </summary>
    public virtual void OnTimerEvent(float progress)
    {
    }

    public virtual void OnInvisibilityStart()
    {
        gunMaterial.SetFloat("_Invisibility", 1);
    }
    public virtual void OnInvisibilityEnd()
    {
        gunMaterial.SetFloat("_Invisibility", 0);
    }


}
