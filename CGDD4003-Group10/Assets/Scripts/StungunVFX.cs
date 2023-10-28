using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StungunVFX : MonoBehaviour
{
    [SerializeField] GameObject baseStungun;
    [SerializeField] GameObject mergedStungun;
    [SerializeField] SpriteRenderer stunMuzzleFlash;

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

    void Init()
    {

    }

    private void Update()
    {
        InvisibilityEffect();
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
