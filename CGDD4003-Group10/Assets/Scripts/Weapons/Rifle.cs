using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Rifle : Weapon
{
    [Header("Weapon Audio")]
    [SerializeField] AudioClip gunshotSFX;
    [SerializeField] AudioSource weaponSound;

    [Header("Rifle Settings")]
    [SerializeField] int maxAmmoCount;
    [SerializeField] float weaponRange;
    [SerializeField] float reloadTime;
    [SerializeField] float camShakeFrequency = 1f;
    [SerializeField] float camShakeDuration = 0.4f;
    [SerializeField] GameObject wallHitEffect;
    [SerializeField] Camera fpsCam;
    [SerializeField] CameraShake camShake;
    [SerializeField] LayerMask targetingMask;
    [SerializeField] TMP_Text ammoCounter;
    [SerializeField] Animator batteryAnimator;
    [SerializeField] Animator reloadingAnimator;
    [SerializeField] Color normalAmmoCounterColor;
    [SerializeField] Color invisibleAmmoCounterColor;

    private float fireRate;
    private float fireRateTimer;

    private int ammoCount;

    public override void OnMouseDownEvent()
    {
        //throw new System.NotImplementedException();
    }

    public override void OnMouseEvent()
    {
        if(fireRateTimer >= fireRate && ammoCount > 0)
        {
            if (camShake)
            {
                camShake.ShakeCamera(camShakeFrequency, 0.5f, camShakeDuration);
            }

            gunAnimator.SetBool("Shooting", true);

            //Insert audio here
            //weaponSound.Stop();
            //weaponSound.PlayOneShot(gunshotSFX);

            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
            RaycastHit hit;

            //Detect hit on enemy
            if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange, targetingMask))
            {
                print("Hit: " + hit.collider.gameObject.name);

                TargetAreaCollider targetAreaCollider = hit.collider.GetComponent<TargetAreaCollider>();
                BossCollider bossCollider = hit.collider.GetComponent<BossCollider>();
                CaptureTentacle captureTentacle = hit.collider.GetComponent<CaptureTentacle>();
                Barrel barrel = hit.collider.GetComponent<Barrel>();

                if (targetAreaCollider != null && captureTentacle == null)
                {
                    Ghost.HitInformation hitInformation = targetAreaCollider.OnShot(weaponInfo.damageMultiplier);
                    Score.AddToScore(Color.gray, (int)((hitInformation.pointWorth + hitInformation.targetArea.pointsAddition) * weaponInfo.scoreMultiplier));

                    SpawnBlood(hitInformation.bigBlood, hitInformation.smallBlood, hitInformation.targetArea.difficulty, hit);
                }
                else if (bossCollider != null)
                {
                    Boss.BossHitInformation hitInformation = bossCollider.boss.GotHit(hit.point, bossCollider.HeadID, weaponInfo.damageMultiplier);
                    if (hitInformation.pointWorth > 0)
                        Score.AddToScore(Color.gray, (int)(hitInformation.pointWorth * weaponInfo.scoreMultiplier));
                }
                else if (captureTentacle != null)
                {
                    captureTentacle.TakeDamage(50);
                }
                else if (barrel != null && (barrel.gameObject.tag == "ExplosiveBarrel" || barrel.gameObject.tag == "ShockBarrel"))
                {
                    barrel.StartExplosion();
                }
                else
                {
                    Instantiate(wallHitEffect, hit.point + hit.normal * 0.1f, Quaternion.identity);
                }
            }

            Score.totalShotsFired++;

            ammoCount--;
            fireRateTimer = 0;

            ammoCounter.text = ammoCount.ToString();

            if (ammoCount == 0)
            {
                StartCoroutine(Reload());
            }
        }
    }

    IEnumerator Reload()
    {
        float reloadTimer = 0;

        gunAnimator.SetBool("Reloading", true);
        gunAnimator.SetBool("Shooting", false);

        print("Reloading Rifle");

        while (reloadTimer < reloadTime)
        {
            yield return null;
            reloadTimer += Time.deltaTime;
            reloadingAnimator.SetFloat("Reloading", reloadTimer / reloadTime);
        }

        gunAnimator.SetBool("Reloading", false);

        print("Reloading Rifle Completed");

        ammoCount = maxAmmoCount;

        ammoCounter.text = ammoCount.ToString();
    }

    public override void OnMouseUpEvent()
    {
        //throw new System.NotImplementedException();
        gunAnimator.SetBool("Shooting", false);
    }

    public override void OnNoMouseEvent()
    {
        //throw new System.NotImplementedException();
    }

    public override void OnPassiveEvent()
    {
        fireRateTimer = Mathf.Clamp(fireRateTimer + Time.deltaTime, 0, fireRate + 0.1f);
    }

    public override void ResetWeapon()
    {
        ammoCount = maxAmmoCount;
        ammoCounter.text = ammoCount.ToString();
        fireRateTimer = 0;
        fireRate = 1 / (2 * weaponInfo.shootSpeed);
    }

    public override void OnTimerEvent(float progress)
    {
        if(batteryAnimator.enabled)
            batteryAnimator.SetFloat("Charge", progress);
    }

    public override void OnInvisibilityStart()
    {
        base.OnInvisibilityStart();

        ammoCounter.color = invisibleAmmoCounterColor;
    }
    public override void OnInvisibilityEnd()
    {
        base.OnInvisibilityEnd();

        ammoCounter.color = normalAmmoCounterColor;
    }
}
