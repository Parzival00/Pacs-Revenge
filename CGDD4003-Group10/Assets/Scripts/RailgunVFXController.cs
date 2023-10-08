using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailgunVFXController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] Animator muzzleFlash;
    [SerializeField] float updateInterval = 0.05f;
    [SerializeField] float chargeBarUpdateInterval = 0.05f;
    [SerializeField] float timerUpdateInterval = 0.5f;
    [SerializeField] float gunTimerOffset = 0.05f;
    [SerializeField] float exhauseEmissionMinIntensity = 0;
    [SerializeField] float exhauseEmissionMaxIntensity = 18;
    [SerializeField] [Range(0,1)] float overheatWarningThreshold = 0.7f;
    [SerializeField] float overheatWarningBlinkInterval = 0.2f;
    //[SerializeField] Color overheatWarningBlinkColor = new Color(245, 22, 22, 255);
    //[SerializeField] Color overheatWarningDefaultColor = new Color(255, 255, 255, 255);
    [SerializeField] float overheatWarningBlinkEmissionIntensity = 1.11f;
    [SerializeField] Transform chargeEffectStart;
    [SerializeField] Transform chargeEffectEnd;
    [SerializeField] Transform[] chargeEffects;

    bool overheatWarningIsBlinking = false;

    [Header("Materials")]
    [SerializeField] Material chargeBarrelMat;
    [SerializeField] Material chasisMat;
    [SerializeField] Material chargeIndicatorMat;
    [SerializeField] Material shotChargeBarMat;
    [SerializeField] Material overheatWarningMat;
    [SerializeField] Material batteryMat;


    // Start is called before the first frame update
    void Start()
    {
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        Init();
    }
    
    void Init()
    {
        chargeBarrelMat.SetFloat("_ChargeAmount", 0);
        chargeBarrelMat.SetFloat("_DechargeAmount", 0);
        chargeBarrelMat.SetFloat("_Decharge", 0);

        chargeIndicatorMat.SetFloat("_ChargeAmount", 1);
        shotChargeBarMat.SetFloat("_ChargeAmount", 1);

        chasisMat.SetFloat("_EmissionIntensity", exhauseEmissionMinIntensity);

        //overheatWarningMat.SetColor("_Color", overheatWarningDefaultColor);
        overheatWarningMat.SetFloat("_EmissionIntensity", 0);

        overheatWarningIsBlinking = false;

        muzzleFlash.gameObject.SetActive(false);

        for (int i = 0; i < chargeEffects.Length; i++)
        {
            chargeEffects[i].gameObject.SetActive(false);
        }
    }

    public void ActivateEffects()
    {
        Init();

        StartCoroutine(GunActive());
        StartCoroutine(GunActiveChargeBar());
        StartCoroutine(GunActiveTimer());
    }

    IEnumerator GunActiveChargeBar()
    {
        WaitForSeconds waitInterval = new WaitForSeconds(chargeBarUpdateInterval);
        while (PlayerController.gunActivated)
        {
            if (!playerController.Overheated)
            {
                shotChargeBarMat.SetFloat("_ChargeAmount", playerController.WeaponCharge);

                if(!overheatWarningIsBlinking && playerController.WeaponTemp01 >= overheatWarningThreshold)
                {
                    StartCoroutine(OverheatWarningBlink());
                } 
                else if(playerController.WeaponTemp01 < overheatWarningThreshold) 
                {
                    overheatWarningMat.SetFloat("_EmissionIntensity", 0);
                }
            } else
            {
                shotChargeBarMat.SetFloat("_ChargeAmount", playerController.WeaponTemp01);
            }

            yield return waitInterval;
        }
    }

    IEnumerator GunActive()
    {
        WaitForSeconds waitInterval = new WaitForSeconds(updateInterval);
        while (PlayerController.gunActivated)
        {
            if (!playerController.Overheated)
            {
                if (playerController.WeaponDecharging)
                {
                    chargeBarrelMat.SetFloat("_Decharge", 1);
                    chargeBarrelMat.SetFloat("_DechargeAmount", playerController.WeaponDecharge);
                    CheckChargeEffects(playerController.WeaponDecharge, true);
                }
                else
                {
                    chargeBarrelMat.SetFloat("_Decharge", 0);

                    CheckChargeEffects(playerController.WeaponCharge, false);
                }
                chargeBarrelMat.SetFloat("_ChargeAmount", playerController.WeaponCharge);
                chasisMat.SetFloat("_EmissionIntensity",
                    exhauseEmissionMinIntensity + playerController.WeaponTemp01 * (exhauseEmissionMaxIntensity - exhauseEmissionMinIntensity));
            }
            else
            {
                chargeBarrelMat.SetFloat("_ChargeAmount", playerController.WeaponTemp01);
                chasisMat.SetFloat("_EmissionIntensity", 
                    exhauseEmissionMinIntensity + playerController.WeaponTemp01 * (exhauseEmissionMaxIntensity - exhauseEmissionMinIntensity));
                CheckChargeEffects(playerController.WeaponTemp01, false);
            }

            yield return waitInterval;
        }
    }

    void CheckChargeEffects(float threshold, bool greaterThan)
    {
        float dstBtwStartEnd = chargeEffectEnd.localPosition.z - chargeEffectStart.localPosition.z;

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
        }
    }

    IEnumerator GunActiveTimer()
    {
        WaitForSeconds waitInterval = new WaitForSeconds(timerUpdateInterval);
        while (PlayerController.gunActivated)
        {
            chargeIndicatorMat.SetFloat("_ChargeAmount", playerController.GunTimer01 + gunTimerOffset);
            yield return waitInterval;
        }

        chargeIndicatorMat.SetFloat("_ChargeAmount", 0);

        yield return new WaitForSeconds(1.5f);

        Init();
    }


    IEnumerator OverheatWarningBlink()
    {
        WaitForSeconds waitInterval = new WaitForSeconds(overheatWarningBlinkInterval);

        overheatWarningIsBlinking = true;

        bool blinkOn = true;
        while (playerController.WeaponTemp01 >= overheatWarningThreshold && PlayerController.gunActivated && !playerController.Overheated)
        {
            if(blinkOn)
            {
                overheatWarningMat.SetFloat("_EmissionIntensity", overheatWarningBlinkEmissionIntensity);
            } 
            else
            {
                overheatWarningMat.SetFloat("_EmissionIntensity", 0);
            }

            blinkOn = !blinkOn;
            yield return waitInterval;
        }

        if(!playerController.Overheated)
            overheatWarningMat.SetFloat("_EmissionIntensity", 0);

        overheatWarningIsBlinking = false;
    }

    public void Shoot(RaycastHit hitInfo, float range)
    {
        muzzleFlash.gameObject.SetActive(true);
        muzzleFlash.SetTrigger("MuzzleFlash");
    }
}
