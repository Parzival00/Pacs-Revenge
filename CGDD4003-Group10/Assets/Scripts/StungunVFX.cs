using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StungunVFX : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] GameObject baseStungun;
    [SerializeField] GameObject mergedStungun;
    [SerializeField] SpriteRenderer stunMuzzleFlash;
    [SerializeField] Color[] fillColors;
    [SerializeField] float canFireEmission = 0.3f;
    [SerializeField] float cantFireEmission = -0.32f;

    [Header("Materials")]
    [SerializeField] Material stunContainerMat;
    [SerializeField] Material stungunMuzzleFlash;

    // Start is called before the first frame update
    void Start()
    {
        baseStungun.SetActive(true);
        mergedStungun.SetActive(false);
        stunMuzzleFlash.gameObject.SetActive(false);

        stungunMuzzleFlash.SetFloat("_Alpha", 1f);
    }

    private void Update()
    {
        InvisibilityEffect();

        if(playerController.StunGunCanFire)
        {
            stunContainerMat.SetFloat("_EmissionIntensity", canFireEmission);
            if(playerController.StunAmmoCount < fillColors.Length)
                stunContainerMat.SetColor("_EmissionColor", fillColors[playerController.StunAmmoCount]);
        } else
        {
            if (playerController.StunAmmoCount < fillColors.Length)
                stunContainerMat.SetColor("_EmissionColor", fillColors[playerController.StunAmmoCount]);

            stunContainerMat.SetFloat("_EmissionIntensity", cantFireEmission);
        }
    }

    bool invisibilityCheck = false;
    void InvisibilityEffect()
    {
        if (invisibilityCheck != PlayerController.invisibilityActivated)
        {
            if (PlayerController.invisibilityActivated)
            {
                baseStungun.SetActive(false);
                mergedStungun.SetActive(true);

                stungunMuzzleFlash.SetFloat("_Alpha", 0.12f);
            }
            else
            {
                baseStungun.SetActive(true);
                mergedStungun.SetActive(false);

                stungunMuzzleFlash.SetFloat("_Alpha", 1f);
            }
        }

        invisibilityCheck = PlayerController.invisibilityActivated;
    }

    public void Shoot()
    {
        StartCoroutine(StunMuzzleFlash());
    }


    IEnumerator StunMuzzleFlash()
    {
        stunMuzzleFlash.gameObject.SetActive(true);
        stunMuzzleFlash.flipX = Random.Range(0, 2) == 1 ? true : false;
        stunMuzzleFlash.flipY = Random.Range(0, 2) == 1 ? true : false;

        yield return new WaitForSeconds(0.2f);

        stunMuzzleFlash.gameObject.SetActive(false);
    }
}
