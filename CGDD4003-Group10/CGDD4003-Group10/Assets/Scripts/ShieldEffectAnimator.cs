using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEffectAnimator : MonoBehaviour
{
    [SerializeField] Material shieldEffect;
    [SerializeField] float activateSpeed = 2;
    [SerializeField] float activateStepSize = 0.1f;
    [SerializeField] float vignetteMin = 0;
    [SerializeField] float vignetteMax = 0.65f;
    [SerializeField] float deactivateSpeed = 20;
    [SerializeField] float deactivateStepSize = 0.1f;
    [SerializeField] float breakSpeed = 10;
    [SerializeField] float breakStepSize = 0.1f;

    [Header("Shield Audio Settings")]
    [SerializeField] AudioSource shieldSource;
    [SerializeField] AudioClip shieldActivate;
    [SerializeField] AudioClip shieldDeactivate;
    [SerializeField] AudioClip shieldBreak;

    bool isAnimating = false;

    // Start is called before the first frame update
    void Start()
    {
        shieldEffect.SetInt("_Active", 0);
        shieldEffect.SetFloat("_ActivateProgress", 0);
        shieldEffect.SetFloat("_BreakProgress", 0);
        shieldEffect.SetFloat("_ShieldVignetteSmoothness", vignetteMax);
        isAnimating = false;
    }

    public void PlayShieldUp()
    {
        StartCoroutine(ShieldUpAnimation());
    }
    IEnumerator ShieldUpAnimation()
    {
        yield return new WaitUntil(() => isAnimating == false);

        if (shieldSource != null)
            shieldSource.PlayOneShot(shieldActivate);

        isAnimating = true;

        float progress = 0;
        shieldEffect.SetFloat("_ActivateProgress", progress);

        shieldEffect.SetFloat("_ShieldVignetteSmoothness", vignetteMin);

        shieldEffect.SetFloat("_BreakProgress", 0);

        shieldEffect.SetInt("_Active", 1);

        WaitForSeconds progressWait = new WaitForSeconds(activateStepSize);

        while (progress < 1)
        {
            shieldEffect.SetFloat("_ActivateProgress", progress);

            //shieldEffect.SetFloat("_ShieldVignetteSmoothness", Mathf.Clamp(progress, vignetteMin, vignetteMax));

            yield return progressWait;

            progress += activateSpeed * activateStepSize * 0.1f;
        }

        progress = 1;

        shieldEffect.SetFloat("_ActivateProgress", progress);
        //shieldEffect.SetFloat("_ShieldVignetteSmoothness", vignetteMax);

        progress = vignetteMin;

        while(progress < vignetteMax)
        {
            shieldEffect.SetFloat("_ShieldVignetteSmoothness", progress);

            yield return progressWait;

            progress += activateSpeed * activateStepSize * 0.1f;
        }

        shieldEffect.SetFloat("_ShieldVignetteSmoothness", vignetteMax);

        isAnimating = false;
    }

    public void PlayShieldDown()
    {
        StartCoroutine(ShieldDownAnimation());
    }
    IEnumerator ShieldDownAnimation()
    {
        yield return new WaitUntil(() => isAnimating == false);

        if (shieldSource != null)
            shieldSource.PlayOneShot(shieldDeactivate);

        isAnimating = true;

        float progress = 1;

        shieldEffect.SetFloat("_ActivateProgress", progress);

        shieldEffect.SetFloat("_ShieldVignetteSmoothness", vignetteMax);

        shieldEffect.SetFloat("_BreakProgress", 0);

        shieldEffect.SetInt("_Active", 1);

        WaitForSeconds progressWait = new WaitForSeconds(deactivateStepSize);

        while (progress > 0)
        {
            shieldEffect.SetFloat("_ActivateProgress", progress);

            yield return progressWait;

            progress -= deactivateSpeed * deactivateStepSize * 0.1f;
        }

        progress = 0;

        shieldEffect.SetFloat("_ActivateProgress", progress);

        shieldEffect.SetInt("_Active", 0);

        isAnimating = false;
    }

    public void PlayShieldBreak()
    {
        StartCoroutine(ShieldBreakAnimation());
    }
    IEnumerator ShieldBreakAnimation()
    {
        yield return new WaitUntil(() => isAnimating == false);

        if (shieldSource != null)
            shieldSource.PlayOneShot(shieldBreak);

        isAnimating = true;

        float progress = 0;

        shieldEffect.SetFloat("_BreakProgress", progress);

        shieldEffect.SetFloat("_ShieldVignetteSmoothness", vignetteMax);

        shieldEffect.SetInt("_Active", 1);

        WaitForSeconds progressWait = new WaitForSeconds(breakStepSize);

        while (progress < 1)
        {
            shieldEffect.SetFloat("_BreakProgress", progress);

            yield return progressWait;

            progress += breakSpeed * breakStepSize * 0.1f;
        }

        progress = 1;

        shieldEffect.SetFloat("_BreakProgress", progress);

        shieldEffect.SetInt("_Active", 0);

        isAnimating = false;
    }

    private void OnApplicationQuit()
    {
        shieldEffect.SetInt("_Active", 0);
        shieldEffect.SetFloat("_ActivateProgress", 0);
        shieldEffect.SetFloat("_BreakProgress", 0);
        shieldEffect.SetFloat("_ShieldVignetteSmoothness", vignetteMax);
        isAnimating = false;
    }
}
