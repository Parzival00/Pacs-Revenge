using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCorruptionEffect : MonoBehaviour
{
    [SerializeField] Material corruptedEffect;
    [SerializeField] Image fullscreenRed;

    // Start is called before the first frame update
    void Start()
    {
        ResetCorruption();
    }

    private void ResetCorruption()
    {
        corruptedEffect.SetFloat("_Threshold", 1.2f);
        corruptedEffect.SetFloat("_Strength", 0.2f);
        corruptedEffect.SetFloat("_Strength2", 0.1f);
        corruptedEffect.SetFloat("_Vignette", 0.01f);

        Color color = fullscreenRed.color;
        color.a = 0;
        fullscreenRed.color = color;
    }

    public void StartCorruption()
    {
        corruptedEffect.SetFloat("_Threshold", 1.0f);
        corruptedEffect.SetFloat("_Strength", 0.2f);
        corruptedEffect.SetFloat("_Strength2", 0.1f);
        corruptedEffect.SetFloat("_Vignette", 0.01f);

        Color color = fullscreenRed.color;
        color.a = 0;
        fullscreenRed.color = color;
    }

    public void ProgressCorruption()
    {
        float currentThreshold = corruptedEffect.GetFloat("_Threshold");
        corruptedEffect.SetFloat("_Threshold", currentThreshold - 0.2f);

        if(currentThreshold < 0.8f)
        {
            float currentStrength = corruptedEffect.GetFloat("_Strength");
            float currentStrength2 = corruptedEffect.GetFloat("_Strength2");
            corruptedEffect.SetFloat("_Strength", Mathf.Clamp01(currentStrength + 0.2f));
            corruptedEffect.SetFloat("_Strength2", Mathf.Clamp01(currentStrength2 + 0.2f));
        }
    }

    public void ProgressCorruptionEnd()
    {
        float currentThreshold = corruptedEffect.GetFloat("_Threshold");
        corruptedEffect.SetFloat("_Threshold", currentThreshold - 0.4f);

        float currentStrength = corruptedEffect.GetFloat("_Strength");
        float currentStrength2 = corruptedEffect.GetFloat("_Strength2");
        corruptedEffect.SetFloat("_Strength", Mathf.Clamp01(currentStrength + 0.2f));
        corruptedEffect.SetFloat("_Strength2", Mathf.Clamp01(currentStrength2 + 0.2f));

        float currentVignette = corruptedEffect.GetFloat("_Vignette");
        corruptedEffect.SetFloat("_Vignette", currentVignette - 0.15f);

        Color color = fullscreenRed.color;
        color.a = Mathf.Clamp01(color.a + 0.2f);
        fullscreenRed.color = color;
    }
}
