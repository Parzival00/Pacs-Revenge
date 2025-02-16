using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Railgun : Weapon
{
    [Header("Weapon Audio")]
    [SerializeField] AudioClip gunshotSFX;
    [SerializeField] AudioClip overheatSFX;
    [SerializeField] AudioClip chargeupSFX;
    [SerializeField] AudioClip chargeReadySFX;
    [SerializeField] AudioSource weaponSound;
    [SerializeField] AudioSource weaponChargeSound;
    [SerializeField] AudioSource weaponChargeReadySound;

    [Header("Railgun Settings")]
    [SerializeField] string railgunAlertMessage = "Advanced Targeting System Activated";
    [SerializeField] float railgunAlertLength = 2;
    [SerializeField] RailgunVFX railGunVFX;
    [SerializeField] GameObject wallHitEffect;
    [SerializeField] Animator railgunAnimator;
    [SerializeField] float chargeTime;
    [SerializeField] float maxWeaponTemp = 5;
    [SerializeField] float overheatSpeed = 1;
    [SerializeField] float cooldownSpeed = 1;
    [SerializeField] float dechargeTime = .25f;
    [SerializeField] Camera fpsCam;
    [SerializeField] float camShakeFrequency = 1f;
    [SerializeField] float camShakeDuration = 0.4f;
    [SerializeField] CameraShake camShake;
    [SerializeField] float weaponRange;
    [SerializeField] LayerMask targetingMask;

    private float weaponCharge;
    private float weaponDecharge;
    private float weaponTemp;
    private bool overheated = false;
    private bool weaponDecharging = false;

    private bool canFire = true;

    bool chargeReady;
    /// <summary>
    /// When the left mouse button is pressed this generates a raycast to where the player was aiming.
    /// A sound effect is played and the raytrace is rendered
    /// </summary>
    void RailgunFire()
    {
        if (canFire == false)
            return;

        if (Input.GetMouseButtonUp(0) && !overheated)
        {
            if (weaponCharge >= 1)
            {
                if (camShake) camShake.ShakeCamera(camShakeFrequency, 0.5f, camShakeDuration);

                /*if (Score.totalShotsFired <= 0 && tutorial)
                {
                    //tutorial.ToggleReleasePrompt(false);
                }*/

                weaponSound.Stop();
                weaponChargeSound.Stop();
                weaponSound.PlayOneShot(gunshotSFX);

                Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
                RaycastHit hit;

                //Detect hit on enemy
                if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange, targetingMask))
                {
                    print("Hit: " + hit.collider.gameObject.name);

                    TargetAreaCollider targetAreaCollider = hit.collider.GetComponent<TargetAreaCollider>();
                    BossCollider bossCollider = hit.collider.GetComponent<BossCollider>();
                    CaptureTentacle captureTentacle = hit.collider.GetComponent<CaptureTentacle>();

                    if (targetAreaCollider != null && captureTentacle == null)
                    {
                        Ghost.HitInformation hitInformation = targetAreaCollider.OnShot(weaponInfo.damageMultiplier);
                        Score.AddToScore(Color.gray, (hitInformation.pointWorth + hitInformation.targetArea.pointsAddition));

                        //SpawnBlood(hitInformation.bigBlood, hitInformation.smallBlood, hitInformation.targetArea.difficulty, hit);
                    }
                    else if (bossCollider != null)
                    {
                        Boss.BossHitInformation hitInformation = bossCollider.boss.GotHit(hit.point, bossCollider.HeadID);
                        if (hitInformation.pointWorth > 0)
                            Score.AddToScore(Color.gray, hitInformation.pointWorth);
                    }
                    else if (captureTentacle != null)
                    {
                        captureTentacle.TakeDamage(50);
                    }
                    else
                    {
                        Instantiate(wallHitEffect, hit.point + hit.normal * 0.1f, Quaternion.identity);
                    }
                }

                railGunVFX.Shoot(hit, weaponRange);
                railgunAnimator.SetTrigger("Shoot");
                Score.totalShotsFired++;
                StartCoroutine(Decharge());
            }
            else
            {
                //weaponChargeSound.Pause();
                weaponChargeSound.pitch = -1;
            }
        }
        else if (Input.GetMouseButtonDown(0) && !overheated)
        {
            weaponChargeSound.pitch = 1;
            weaponChargeSound.time = Mathf.Clamp01(weaponCharge);
            weaponChargeSound.clip = chargeupSFX;
            weaponChargeSound.Play();

            chargeReady = false;

            /*if (Score.totalShotsFired <= 0 && tutorial)
            {
                //tutorial.ToggleShootPrompt(false);
            }*/
        }
        else if (Input.GetMouseButton(0) && !overheated)
        {
            if (chargeReady == true && weaponCharge < 1f)
            {
                weaponChargeSound.pitch = 1;
                weaponChargeSound.time = Mathf.Clamp01(weaponCharge);
                weaponChargeSound.clip = chargeupSFX;
                weaponChargeSound.Play();

                chargeReady = false;
            }

            if (weaponCharge < 1f)
            {
                weaponCharge += (Time.deltaTime * (1 / chargeTime));
            }
            if (weaponCharge >= 1f)
            {
                if (chargeReady == false)
                {
                    weaponChargeReadySound.PlayOneShot(chargeReadySFX);
                    chargeReady = true;
                }
                /*if (doTutorials && Score.totalShotsFired <= 0 && tutorial)
                {
                    //tutorial.ToggleReleasePrompt(true);
                }*/
            }

            weaponTemp += (Time.deltaTime * (overheatSpeed));
        }
        else if (!Input.GetMouseButton(0) && !overheated)
        {
            if (weaponCharge > 0)
            {
                weaponCharge -= (Time.deltaTime * (1 / (chargeTime / 2)));
                weaponTemp -= Time.deltaTime * (overheatSpeed * 2);
            }
            else
            {
                weaponTemp = 0;
                weaponCharge = 0;
            }
        }
    }

    /// <summary>
    /// Decharges the gun and acts as a shot cooldown
    /// </summary>
    /// <returns></returns>
    private IEnumerator Decharge()
    {
        canFire = false;
        weaponCharge = 1;
        weaponDecharge = 0;

        weaponDecharging = true;

        while (weaponDecharge <= 1)
        {
            weaponCharge -= (Time.deltaTime * (1 / dechargeTime));
            weaponDecharge += (Time.deltaTime * (1 / dechargeTime));

            yield return null;
        }

        weaponDecharging = false;

        weaponCharge = 0;
        weaponDecharge = 1;
        weaponTemp = 0f;
        canFire = true;
    }

    /// <summary>
    /// Handles the changes to the charge UI and the sound effect for weapon overheating. Also decreases the temperature over time
    /// </summary>
    void CheckWeaponTemp()
    {
        if (weaponTemp >= maxWeaponTemp && !overheated)
        {
            /*if (doTutorials && Score.timesOverheated <= 1 && tutorial)
            {
                //tutorial.ToggleReleasePrompt(false);
                //tutorial.ToggleOverheatPrompt(true);
            }*/

            Score.timesOverheated++;

            overheated = true;
            //weaponSound.volume = .9f;
            weaponSound.PlayOneShot(overheatSFX);
            //weaponSound.volume = .1f;
        }
        else if (weaponTemp <= 0f && overheated)
        {
            weaponTemp = 0f;
            weaponCharge = 0f;
            overheated = false;
        }
        else if (overheated && !Input.GetMouseButton(0))
        {
            /*if (doTutorials && Score.timesOverheated <= 2 && tutorial)
            {
                //tutorial.ToggleOverheatPrompt(false);
            }*/

            weaponTemp -= Time.deltaTime * cooldownSpeed;
        }
    }

    public override void OnMouseDownEvent()
    {
        if (!overheated)
        {
            weaponChargeSound.pitch = 1;
            weaponChargeSound.time = Mathf.Clamp01(weaponCharge);
            weaponChargeSound.clip = chargeupSFX;
            weaponChargeSound.Play();

            chargeReady = false;

            /*if (Score.totalShotsFired <= 0 && tutorial)
            {
                //tutorial.ToggleShootPrompt(false);
            }*/
        }
    }

    public override void OnMouseEvent()
    {
        if (!overheated)
        {
            if (chargeReady == true && weaponCharge < 1f)
            {
                weaponChargeSound.pitch = 1;
                weaponChargeSound.time = Mathf.Clamp01(weaponCharge);
                weaponChargeSound.clip = chargeupSFX;
                weaponChargeSound.Play();

                chargeReady = false;
            }

            if (weaponCharge < 1f)
            {
                weaponCharge += (Time.deltaTime * (1 / chargeTime));
            }
            if (weaponCharge >= 1f)
            {
                if (chargeReady == false)
                {
                    weaponChargeReadySound.PlayOneShot(chargeReadySFX);
                    chargeReady = true;
                }
                /*if (doTutorials && Score.totalShotsFired <= 0 && tutorial)
                {
                    //tutorial.ToggleReleasePrompt(true);
                }*/
            }

            weaponTemp += (Time.deltaTime * (overheatSpeed));
        }
    }

    public override void OnMouseUpEvent()
    {
        if (!overheated)
        {
            if (weaponCharge >= 1)
            {
                if (camShake) camShake.ShakeCamera(camShakeFrequency, 0.5f, camShakeDuration);

                /*if (Score.totalShotsFired <= 0 && tutorial)
                {
                    //tutorial.ToggleReleasePrompt(false);
                }*/

                weaponSound.Stop();
                weaponChargeSound.Stop();
                weaponSound.PlayOneShot(gunshotSFX);

                Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
                RaycastHit hit;

                //Detect hit on enemy
                if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange, targetingMask))
                {
                    print("Hit: " + hit.collider.gameObject.name);

                    TargetAreaCollider targetAreaCollider = hit.collider.GetComponent<TargetAreaCollider>();
                    BossCollider bossCollider = hit.collider.GetComponent<BossCollider>();
                    CaptureTentacle captureTentacle = hit.collider.GetComponent<CaptureTentacle>();

                    if (targetAreaCollider != null && captureTentacle == null)
                    {
                        Ghost.HitInformation hitInformation = targetAreaCollider.OnShot(weaponInfo.damageMultiplier);
                        Score.AddToScore(Color.gray, (hitInformation.pointWorth + hitInformation.targetArea.pointsAddition));

                        //SpawnBlood(hitInformation.bigBlood, hitInformation.smallBlood, hitInformation.targetArea.difficulty, hit);
                    }
                    else if (bossCollider != null)
                    {
                        Boss.BossHitInformation hitInformation = bossCollider.boss.GotHit(hit.point, bossCollider.HeadID, weaponInfo.damageMultiplier);
                        if (hitInformation.pointWorth > 0)
                            Score.AddToScore(Color.gray, hitInformation.pointWorth);
                    }
                    else if (captureTentacle != null)
                    {
                        captureTentacle.TakeDamage(50);
                    }
                    else
                    {
                        Instantiate(wallHitEffect, hit.point + hit.normal * 0.1f, Quaternion.identity);
                    }
                }

                railGunVFX.Shoot(hit, weaponRange);
                railgunAnimator.SetTrigger("Shoot");
                Score.totalShotsFired++;
                StartCoroutine(Decharge());
            }
            else
            {
                //weaponChargeSound.Pause();
                weaponChargeSound.pitch = -1;
            }
        }
    }

    public override void OnPassiveEvent()
    {
        CheckWeaponTemp();
    }
}
