using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] Image shieldDamageImage;
    [SerializeField] Image shieldIncreaseImage;
    [SerializeField] float flashSpeed = 0.5f;

    [Header("Shield Audio Settings")]
    [SerializeField] AudioSource shieldSource;
    [SerializeField] AudioClip shieldActivate;
    [SerializeField] AudioClip shieldDeactivate;
    [SerializeField] AudioClip shieldBreak_Full;
    [SerializeField] AudioClip shieldBreak_Partial;

    bool isAnimating = false;

    // Start is called before the first frame update
    void Start()
    {
        shieldEffect.SetInt("_Active", 0);
        shieldEffect.SetFloat("_ActivateProgress", 0);
        shieldEffect.SetFloat("_BreakProgress", 0);
        shieldEffect.SetFloat("_ShieldVignetteSmoothness", vignetteMax);
        isAnimating = false;

        shieldIncreaseImage.gameObject.SetActive(false);
        shieldDamageImage.gameObject.SetActive(false);
    }

    public void PlayShieldUp()
    {
        StartCoroutine(ShieldUpAnimation());
    }
    IEnumerator ShieldUpAnimation()
    {
        yield return new WaitUntil(() => isAnimating == false);

        if (shieldSource != null)
        {
            shieldSource.PlayOneShot(shieldActivate);
        }

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
    public void PlayExtraShieldUp()
    {
        StartCoroutine(ExtraShieldFlash());
    }
    IEnumerator ExtraShieldFlash()
    {
        shieldDamageImage.gameObject.SetActive(false);
        shieldIncreaseImage.gameObject.SetActive(true);

        float alpha = 0;
        Color color = shieldIncreaseImage.color;
        color.a = alpha;
        shieldIncreaseImage.color = color;

        float change = 1 / (flashSpeed / 2);
        while (alpha < .75f)
        {
            alpha += change * Time.deltaTime;
            print(alpha);
            color = shieldIncreaseImage.color;
            color.a = alpha;
            shieldIncreaseImage.color = color;
            yield return null;
        }

        alpha = 0.75f;
        color = shieldIncreaseImage.color;
        color.a = alpha;
        shieldIncreaseImage.color = color;

        while (alpha > 0)
        {
            alpha -= change * Time.deltaTime;
            color = shieldIncreaseImage.color;
            color.a = alpha;
            shieldIncreaseImage.color = color;
            yield return null;
        }

        alpha = 0;
        color = shieldIncreaseImage.color;
        color.a = alpha;
        shieldIncreaseImage.color = color;
        shieldIncreaseImage.gameObject.SetActive(false);
    }

    public void PlayShieldDown()
    {
        StartCoroutine(ShieldDownAnimation());
    }
    IEnumerator ShieldDownAnimation()
    {
        yield return new WaitUntil(() => isAnimating == false);

        if (shieldSource != null)
        {
            shieldSource.PlayOneShot(shieldDeactivate);
        }

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
        {
            shieldSource.PlayOneShot(shieldBreak_Full);
        }

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

    public void PlayShieldBreakPartial()
    {
        if (shieldSource != null)
        {
            shieldSource.PlayOneShot(shieldBreak_Partial);
        }
        StartCoroutine(DamageFlash());
    }
    public void PlayDamageFlash()
    {
        StartCoroutine(DamageFlash());
    }
    IEnumerator DamageFlash()
    {
        shieldIncreaseImage.gameObject.SetActive(false);
        shieldDamageImage.gameObject.SetActive(true);

        float alpha = 0;
        Color color = shieldDamageImage.color;
        color.a = alpha;
        shieldDamageImage.color = color;

        float change = 1 / (flashSpeed / 2);
        while (alpha < .75f)
        {
            alpha += change * Time.deltaTime;
            print(alpha);
            color = shieldDamageImage.color;
            color.a = alpha;
            shieldDamageImage.color = color;
            yield return null;
        }

        alpha = 0.75f;
        color = shieldDamageImage.color;
        color.a = alpha;
        shieldDamageImage.color = color;

        while (alpha > 0)
        {
            alpha -= change * Time.deltaTime;
            color = shieldDamageImage.color;
            color.a = alpha;
            shieldDamageImage.color = color;
            yield return null;
        }

        alpha = 0;
        color = shieldDamageImage.color;
        color.a = alpha;
        shieldDamageImage.color = color;
        shieldDamageImage.gameObject.SetActive(false);
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
