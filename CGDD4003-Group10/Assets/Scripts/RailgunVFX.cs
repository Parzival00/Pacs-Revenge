using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailgunVFX : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] Railgun railgun;

    [Header("Settings")]
    [SerializeField] [Range(0, 1)] float overheatWarningThreshold = 0.7f;
    [SerializeField] float overheatWarningBlinkInterval = 0.2f;

    [Header("References")]
    //[SerializeField] SpriteRenderer chargeTimeSprite1;
    //[SerializeField] SpriteRenderer chargeTimeSprite2;
    [SerializeField] SpriteRenderer overheatSprite1;
    [SerializeField] SpriteRenderer overheatSprite2;
    [SerializeField] Animator chargeTimerAnimator;
    [SerializeField] Animator railgunAnimator;
    [SerializeField] ParticleSystem overheatSmoke1;
    [SerializeField] ParticleSystem overheatSmoke2;

    /*[Header("Old Settings")]
    [SerializeField] Animator muzzleFlash;
    [SerializeField] float updateInterval = 0.05f;
    [SerializeField] float chargeBarUpdateInterval = 0.05f;
    [SerializeField] float timerUpdateInterval = 0.5f;
    [SerializeField] float gunTimerOffset = 0.05f;
    [SerializeField] float exhauseEmissionMinIntensity = 0;
    [SerializeField] float exhauseEmissionMaxIntensity = 18;
    [SerializeField] [Range(0,1)] float overheatWarningThreshold = 0.7f;
    [SerializeField] float overheatWarningBlinkInterval = 0.2f;
    [SerializeField] float overheatWarningBlinkEmissionIntensity = 1.11f;
    [SerializeField] Transform chargeEffectStart;
    [SerializeField] Transform chargeEffectEnd;
    [SerializeField] Transform[] chargeEffects;
    [SerializeField] GameObject baseRailgun;
    [SerializeField] GameObject mergedRailgun;*/

    bool overheatWarningIsBlinking = false;

    [Header("Materials")]
    [SerializeField] Material railgunMat;
    /*[SerializeField] Material chargeBarrelMat;
    [SerializeField] Material chasisMat;
    [SerializeField] Material chargeIndicatorMat;
    [SerializeField] Material shotChargeBarMat;
    [SerializeField] Material overheatWarningMat;
    [SerializeField] Material batteryMat;
    [SerializeField] Material chargeBarPanel;
    [SerializeField] Material railgunChargeIndicatorPanel;
    [SerializeField] Material chargeEffectMat;
    [SerializeField] Material railgunMuzzleFlash;*/


    // Start is called before the first frame update
    void Start()
    {
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        /*Init();

        baseRailgun.SetActive(true);
        mergedRailgun.SetActive(false);
        chargeIndicatorMat.SetFloat("_Alpha", 1);
        shotChargeBarMat.SetFloat("_Alpha", 1);
        overheatWarningMat.SetFloat("_Alpha", 1);
        chargeEffectMat.SetFloat("_Alpha", 1);
        chargeBarPanel.SetFloat("_Alpha", 1);
        railgunChargeIndicatorPanel.SetFloat("_Alpha", 1);
        railgunMuzzleFlash.SetFloat("_Alpha", 1f);*/
    }


    void Init()
    {
        /*chargeBarrelMat.SetFloat("_ChargeAmount", 0);
        chargeBarrelMat.SetFloat("_DechargeAmount", 0);
        chargeBarrelMat.SetFloat("_Decharge", 0);

        chargeIndicatorMat.SetFloat("_ChargeAmount", 1);
        shotChargeBarMat.SetFloat("_ChargeAmount", 1);

        chasisMat.SetFloat("_EmissionIntensity", exhauseEmissionMinIntensity);

        overheatWarningIsBlinking = false;

        muzzleFlash.gameObject.SetActive(false);

        overheatSmoke1.Stop();
        overheatSmoke2.Stop();

        for (int i = 0; i < chargeEffects.Length; i++)
        {
            chargeEffects[i].gameObject.SetActive(false);
        }*/
    }

    private void Update()
    {
        //InvisibilityEffect();

        if (PlayerController.gunActivated)
        {
            railgunAnimator.SetFloat("Charge", railgun.WeaponCharge);
            chargeTimerAnimator.SetFloat("Timer", 1 - playerController.GunTimer01);

            if (!overheatWarningIsBlinking && railgun.WeaponTemp01 >= overheatWarningThreshold)
            {
                StartCoroutine(OverheatWarningBlink());
            }

            if (railgun.Overheated)
            {
                if (!overheatSmoke1.isPlaying)
                {
                    overheatSmoke1.Play();
                }
                if (!overheatSmoke2.isPlaying)
                {
                    overheatSmoke2.Play();
                }
            }
            else
            {
                if (overheatSmoke1.isPlaying)
                {
                    overheatSmoke1.Stop();
                }
                if (overheatSmoke2.isPlaying)
                {
                    overheatSmoke2.Stop();
                    var main = overheatSmoke2.main;
                }
            }

            if (PlayerController.invisibilityActivated)
            {
                railgunMat.SetFloat("_Invisibility", 1);
            }
            else
            {
                railgunMat.SetFloat("_Invisibility", 0);
            }
        }
    }

   /* void InvisibilityEffect()
    {
        if (!PlayerController.gunActivated)
            return;

        *//*if (PlayerController.invisibilityActivated)
        {
            baseRailgun.SetActive(false);
            mergedRailgun.SetActive(true);

            chargeIndicatorMat.SetFloat("_Alpha", 0.02f);
            shotChargeBarMat.SetFloat("_Alpha", 0.02f);
            overheatWarningMat.SetFloat("_Alpha", 0.12f);
            chargeEffectMat.SetFloat("_Alpha", 0.15f);
            chargeBarPanel.SetFloat("_Alpha", 0.12f);
            railgunChargeIndicatorPanel.SetFloat("_Alpha", 0.12f);
            railgunMuzzleFlash.SetFloat("_Alpha", 0.12f);
        } 
        else
        {
            baseRailgun.SetActive(true);
            mergedRailgun.SetActive(false);

            chargeIndicatorMat.SetFloat("_Alpha", 1);
            shotChargeBarMat.SetFloat("_Alpha", 1);
            overheatWarningMat.SetFloat("_Alpha", 1);
            chargeEffectMat.SetFloat("_Alpha", 1);
            chargeBarPanel.SetFloat("_Alpha", 1);
            railgunChargeIndicatorPanel.SetFloat("_Alpha", 1);
            railgunMuzzleFlash.SetFloat("_Alpha", 1f);
        }*//*
    }*/

    /*public void ActivateEffects()
    {

        //Init();

        //StartCoroutine(GunActive());
        //StartCoroutine(GunActiveChargeBar());
        //StartCoroutine(GunActiveTimer());
    }*/

   /* IEnumerator GunActiveChargeBar()
    {
        WaitForSeconds waitInterval = new WaitForSeconds(chargeBarUpdateInterval);
        while (PlayerController.gunActivated)
        {
            *//*if (!railgun.Overheated)
            {
                shotChargeBarMat.SetFloat("_ChargeAmount", railgun.WeaponCharge);
                
                if (!overheatWarningIsBlinking && railgun.WeaponTemp01 >= overheatWarningThreshold)
                {
                    StartCoroutine(OverheatWarningBlink());
                } 
                else if(railgun.WeaponTemp01 < overheatWarningThreshold) 
                {
                    overheatWarningMat.SetFloat("_EmissionIntensity", 0);
                }
            } else
            {
                shotChargeBarMat.SetFloat("_ChargeAmount", railgun.WeaponTemp01);
            }*//*

            yield return waitInterval;
        }
    }*/

    /*IEnumerator GunActive()
    {
        WaitForSeconds waitInterval = new WaitForSeconds(updateInterval);
        while (PlayerController.gunActivated)
        {
            *//*if (!railgun.Overheated)
            {
                if (railgun.WeaponDecharging)
                {
                    chargeBarrelMat.SetFloat("_Decharge", 1);
                    chargeBarrelMat.SetFloat("_DechargeAmount", railgun.WeaponDecharge);
                    CheckChargeEffects(railgun.WeaponDecharge, true);

                    chasisMat.SetFloat("_EmissionIntensity",
                        exhauseEmissionMinIntensity + Mathf.Min(railgun.WeaponTemp01, railgun.WeaponCharge) * (exhauseEmissionMaxIntensity - exhauseEmissionMinIntensity));
                }
                else
                {
                    chargeBarrelMat.SetFloat("_Decharge", 0);

                    CheckChargeEffects(railgun.WeaponCharge, false);

                    chasisMat.SetFloat("_EmissionIntensity",
                        exhauseEmissionMinIntensity + railgun.WeaponTemp01 * (exhauseEmissionMaxIntensity - exhauseEmissionMinIntensity));
                }
                chargeBarrelMat.SetFloat("_ChargeAmount", railgun.WeaponCharge);


                if (overheatSmoke1.isPlaying)
                {
                    overheatSmoke1.Stop();
                    var main = overheatSmoke1.main;
                }
                if (overheatSmoke2.isPlaying)
                {
                    overheatSmoke2.Stop();
                    var main = overheatSmoke2.main;
                }
            }
            else
            {
                chargeBarrelMat.SetFloat("_ChargeAmount", railgun.WeaponTemp01);
                chasisMat.SetFloat("_EmissionIntensity", 
                    exhauseEmissionMinIntensity + railgun.WeaponTemp01 * (exhauseEmissionMaxIntensity - exhauseEmissionMinIntensity));
                CheckChargeEffects(railgun.WeaponTemp01, false);

                if (!overheatSmoke1.isPlaying)
                {
                    overheatSmoke1.Play();
                    var main = overheatSmoke1.main;
                }
                if (!overheatSmoke2.isPlaying)
                {
                    overheatSmoke2.Play();
                    var main = overheatSmoke2.main;
                }
            }*//*

            yield return waitInterval;
        }
        print("Gun Active");
    }*/

    /*void CheckChargeEffects(float threshold, bool greaterThan)
    {
        *//*float dstBtwStartEnd = chargeEffectEnd.localPosition.z - chargeEffectStart.localPosition.z;

        for (int i = 0; i < chargeEffects.Length; i++)
        {
            float posAlongBarrel = (chargeEffects[i].localPosition.z - chargeEffectStart.localPosition.z) / dstBtwStartEnd;
            if (greaterThan)
            {
                if (posAlongBarrel >= threshold)
                {
                    chargeEffects[i].gameObject.SetActive(true);
                }
                else
                {
                    chargeEffects[i].gameObject.SetActive(false);
                }
            }
            else
            {
                if (posAlongBarrel <= threshold)
                {
                    chargeEffects[i].gameObject.SetActive(true);
                }
                else
                {
                    chargeEffects[i].gameObject.SetActive(false);
                }
            }
        }*//*
    }*/

    /*IEnumerator GunActiveTimer()
    {
        WaitForSeconds waitInterval = new WaitForSeconds(timerUpdateInterval);
        while (PlayerController.gunActivated)
        {
            print("Gun Timer going");
            chargeIndicatorMat.SetFloat("_ChargeAmount", playerController.GunTimer01 + gunTimerOffset);
            yield return waitInterval;
        }

        chargeIndicatorMat.SetFloat("_ChargeAmount", 0);

        yield return new WaitForSeconds(1.5f);

        Init();
    }*/


    IEnumerator OverheatWarningBlink()
    {
        WaitForSeconds waitInterval = new WaitForSeconds(overheatWarningBlinkInterval);

        overheatWarningIsBlinking = true;

        bool blinkOn = true;
        while (railgun.WeaponTemp01 >= overheatWarningThreshold && PlayerController.gunActivated && !railgun.Overheated)
        {
            if(blinkOn)
            {
                overheatSprite1.enabled = true;
                overheatSprite2.enabled = true;
                //overheatWarningMat.SetFloat("_EmissionIntensity", overheatWarningBlinkEmissionIntensity);
            } 
            else
            {
                overheatSprite1.enabled = false;
                overheatSprite2.enabled = false;
                //overheatWarningMat.SetFloat("_EmissionIntensity", 0);
            }

            blinkOn = !blinkOn;
            yield return waitInterval;
        }

        if (!railgun.Overheated)
        {
            overheatSprite1.enabled = false;
            overheatSprite2.enabled = false;
            //overheatWarningMat.SetFloat("_EmissionIntensity", 0);
        }

        overheatWarningIsBlinking = false;
    }

   /* public void Shoot(RaycastHit hitInfo, float range)
    {
        //muzzleFlash.gameObject.SetActive(true);
        //muzzleFlash.SetTrigger("MuzzleFlash");
    }*/
}
