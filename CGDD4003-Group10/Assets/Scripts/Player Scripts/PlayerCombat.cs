using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] TargetOutlineController targetOutlineController;
    [SerializeField] StungunController stungunController;
    [SerializeField] PlayerStateManager playerState;

    [Header("Weapon Audio")]
    [SerializeField] AudioClip stunShotSFX;
    [SerializeField] AudioClip stunShotEmpty;
    [SerializeField] AudioSource weaponSound;

    [Header("Weapon Settings")]
    [SerializeField] WeaponInfo[] weaponInfos;
    [SerializeField] Weapon[] weapons;
    [SerializeField] string railgunAlertMessage = "Advanced Targeting System Activated";
    [SerializeField] float railgunAlertLength = 2;
    [SerializeField] float gunTimeAmount = 5f;
    [SerializeField] Camera fpsCam;
    [SerializeField] float camShakeFrequency = 1f;
    [SerializeField] float camShakeDuration = 0.4f;
    [SerializeField] CameraShake camShake;
    [SerializeField] float weaponRange;
    [SerializeField] LayerMask targetingMask;
    [SerializeField] Color targetingColor = Color.yellow;

    CorruptedGunController corruptedGunController;
    Weapon currentWeapon;

    public Weapon CurrentWeapon => currentWeapon;

    private int weaponIndex;

    //State bools
    bool canFire;
    bool gunActivated;
    bool holdingFire;

    public CameraShake CamShake { get => camShake; }

    public event Action<Color> OnTargetChanged;

    // Start is called before the first frame update
    void Start()
    {
        weaponIndex = PlayerPrefs.GetInt("Weapon");
        currentWeapon = weapons[weaponIndex];
    }

    // Update is called once per frame
    void Update()
    {
        if (canFire)
        {
            if (gunActivated)
            {
                OutlineTargetEnemy();
                currentWeapon.OnPassiveEvent();

                if (holdingFire)
                {
                    currentWeapon.OnMouseEvent();
                }
                else
                {
                    currentWeapon.OnNoMouseEvent();
                }
            }
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (!MainMenuManager.isGamePaused)
        {
            if (gunActivated)
            {
                if (context.phase == InputActionPhase.Started)
                {
                    holdingFire = true;

                    if (!MainMenuManager.isGamePaused && canFire && gunActivated)
                    {
                        currentWeapon.OnMouseDownEvent();
                    }
                }
                else if (context.phase == InputActionPhase.Performed)
                {
                    holdingFire = false;

                    if (!MainMenuManager.isGamePaused && canFire && gunActivated)
                    {
                        currentWeapon.OnMouseUpEvent();
                    }
                }
            }
            else
            {
                if (context.phase == InputActionPhase.Started)
                {
                    stungunController.StungunFire();
                }
            }
        }
    }

    /// <summary>
    /// Activates the corresponding outline for targeted area
    /// </summary>
    private void OutlineTargetEnemy()
    {
        Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit hit;

        //Detect hit on enemy
        if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange, targetingMask))
        {
            //print("Targetting: " + hit.collider.gameObject.name);

            TargetAreaCollider targetAreaCollider = hit.collider.GetComponent<TargetAreaCollider>();
            BossCollider bossCollider = hit.collider.GetComponent<BossCollider>();

            if (targetAreaCollider != null)
            {
                TargetAreaCollider.TargetInfo target = targetAreaCollider.OnTarget();
                targetOutlineController.SetTargetOutline(target);
                OnTargetChanged?.Invoke(targetingColor);
            }
            else if (bossCollider != null && bossCollider.boss.CheckHeadDamagable(bossCollider.HeadID))
            {
                TargetAreaCollider.TargetInfo target = bossCollider.OnTarget();
                targetOutlineController.SetTargetOutline(target);
                OnTargetChanged?.Invoke(targetingColor);
            }
            else
            {
                OnTargetChanged?.Invoke(Color.white);
                targetOutlineController.DeactivateOutline();
            }
        }
        else
        {
            //print("Not Targetting Anything");
            OnTargetChanged?.Invoke(Color.white);
            targetOutlineController.DeactivateOutline();
        }
    }

    //Event Handlers
    private void HandleFireState(bool canFire)
    {
        this.canFire = canFire;
    }
    private void HandleGunState(bool gunActivated)
    {
        if (!gunActivated)
        {
            holdingFire = false;
            currentWeapon.ResetWeapon();

            StartCoroutine(DeactivateGun());
        }
        else
        {
            StartCoroutine(ActivateGun());
        }
    }
    IEnumerator ActivateGun()
    {
        stungunController.HandleGunState(true);

        yield return new WaitForSeconds(0.3f);

        currentWeapon.gameObject.SetActive(true);

        currentWeapon.GunAnimator.ResetTrigger("Unequip");
        currentWeapon.GunAnimator.SetTrigger("Equip");

        yield return new WaitForSeconds(0.3f);

        gunActivated = true;
    }

    IEnumerator DeactivateGun()
    {
        gunActivated = false;

        currentWeapon.GunAnimator.ResetTrigger("Unequip");
        currentWeapon.GunAnimator.SetTrigger("Equip");

        yield return new WaitForSeconds(0.5f);

        stungunController.HandleGunState(false);

        currentWeapon.gameObject.SetActive(false);
    }

    //Subscribe and Unsubscribe to events
    private void OnEnable()
    {
        playerState.OnGunStateChanged += HandleGunState;
        playerState.OnFireStateChanged += HandleFireState;
    }
    private void OnDisable()
    {
        playerState.OnGunStateChanged -= HandleGunState;
        playerState.OnFireStateChanged -= HandleFireState;
    }
}
