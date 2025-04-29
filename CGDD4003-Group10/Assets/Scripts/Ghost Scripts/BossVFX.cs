using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossVFX : MonoBehaviour
{
    [Header("Aura Settings")]
    [SerializeField] SpriteRenderer auraSpriteRenderer;
    Material auraMaterial;
    [SerializeField] float auraDissolveLength = 0.5f;
    [SerializeField] int frameRate = 8;
    [SerializeField] ParticleSystem enrageParticleSystem;
    [SerializeField] Texture northAuraNoise;
    [SerializeField] Texture northeastAuraNoise;
    [SerializeField] Texture northwestAuraNoise;
    [SerializeField] Texture eastAuraNoise;
    [SerializeField] Texture westAuraNoise;
    [SerializeField] Texture slamAuraNoise;

    [Header("Pulse Settings")]
    [SerializeField] SpriteRenderer bossRenderer;
    Material bossMaterial;
    [Header("Blinky Head")]
    [SerializeField] Texture blinkyNorthMaskTex;
    [SerializeField] Texture blinkyNortheastMaskTex;
    [SerializeField] Texture blinkyNorthwestMaskTex;
    [SerializeField] Texture blinkyEastMaskTex;
    [SerializeField] Texture blinkyWestMaskTex;
    [SerializeField] Texture blinkyAttackMaskTex;
    [SerializeField] Texture blinkySlamMaskTex;
    [Header("Inky Head")]
    [SerializeField] Texture inkyNorthMaskTex;
    [SerializeField] Texture inkyNortheastMaskTex;
    [SerializeField] Texture inkyNorthwestMaskTex;
    [SerializeField] Texture inkyEastMaskTex;
    [SerializeField] Texture inkyWestMaskTex;
    [SerializeField] Texture inkyAttackMaskTex;
    [SerializeField] Texture inkySlamMaskTex;
    [Header("Pinky Head")]
    [SerializeField] Texture pinkyNorthMaskTex;
    [SerializeField] Texture pinkyNortheastMaskTex;
    [SerializeField] Texture pinkyNorthwestMaskTex;
    [SerializeField] Texture pinkyEastMaskTex;
    [SerializeField] Texture pinkyWestMaskTex;
    [SerializeField] Texture pinkyAttackMaskTex;
    [SerializeField] Texture pinkySlamMaskTex;
    [Header("Clyde Head")]
    [SerializeField] Texture clydeNorthMaskTex;
    [SerializeField] Texture clydeNortheastMaskTex;
    [SerializeField] Texture clydeNorthwestMaskTex;
    [SerializeField] Texture clydeEastMaskTex;
    [SerializeField] Texture clydeWestMaskTex;
    [SerializeField] Texture clydeAttackMaskTex;
    [SerializeField] Texture clydeSlamMaskTex;

    // Start is called before the first frame update
    void Awake()
    {
        if (auraSpriteRenderer == null || enrageParticleSystem == null) Debug.LogError("Boss VFX not properly set. Check Inspector");
        auraMaterial = auraSpriteRenderer.sharedMaterial;
        bossMaterial = bossRenderer.sharedMaterial;
    }

    public void DissolveAura()
    {
        StartCoroutine(ChangeAura(true));
    }
    public void MaterializeAura()
    {
        StartCoroutine(ChangeAura(false));
    }
    
    IEnumerator ChangeAura(bool dissolve)
    {
        WaitForSeconds wait = new WaitForSeconds(1f / frameRate);

        float t = dissolve ? 0 : 1;
        
        if(dissolve)
        {
            while(t < 1)
            {
                auraMaterial.SetFloat("_DissolveAmount", t);
                yield return wait;
                t += 1f / frameRate * (1 / auraDissolveLength);
            }
            auraMaterial.SetFloat("_DissolveAmount", 1);
        } else
        {
            while (t > 0 )
            {
                auraMaterial.SetFloat("_DissolveAmount", t);
                yield return wait;
                t -= 1f / frameRate * (1 / auraDissolveLength);
            }
            auraMaterial.SetFloat("_DissolveAmount", 0);
        }
    }

    public void StartPS()
    {
        enrageParticleSystem.Play();
    }

    public void StopPS()
    {
        enrageParticleSystem.Stop();
    }

    public void SetNorthAura()
    {
        auraMaterial.SetTexture("_NoiseMask", northAuraNoise);
    }
    public void SetNortheastAura()
    {
        auraMaterial.SetTexture("_NoiseMask", northeastAuraNoise);
    }
    public void SetNorthwestAura()
    {
        auraMaterial.SetTexture("_NoiseMask", northwestAuraNoise);
    }
    public void SetEastAura()
    {
        auraMaterial.SetTexture("_NoiseMask", eastAuraNoise);
    }
    public void SetWestAura()
    {
        auraMaterial.SetTexture("_NoiseMask", westAuraNoise);
    }
    public void SetSlamAura()
    {
        auraMaterial.SetTexture("_NoiseMask", slamAuraNoise);
    }

    public void SetNorthMask()
    {
        bossMaterial.SetTexture("_BlinkyPulseMask", blinkyNorthMaskTex);
        bossMaterial.SetTexture("_InkyPulseMask", inkyNorthMaskTex);
        bossMaterial.SetTexture("_PinkyPulseMask", pinkyNorthMaskTex);
        bossMaterial.SetTexture("_ClydePulseMask", clydeNorthMaskTex);
    }
    public void SetNortheastMask()
    {
        bossMaterial.SetTexture("_BlinkyPulseMask", blinkyNortheastMaskTex);
        bossMaterial.SetTexture("_InkyPulseMask", inkyNortheastMaskTex);
        bossMaterial.SetTexture("_PinkyPulseMask", pinkyNortheastMaskTex);
        bossMaterial.SetTexture("_ClydePulseMask", clydeNortheastMaskTex);
    }
    public void SetNorthwestMask()
    {
        bossMaterial.SetTexture("_BlinkyPulseMask", blinkyNorthwestMaskTex);
        bossMaterial.SetTexture("_InkyPulseMask", inkyNorthwestMaskTex);
        bossMaterial.SetTexture("_PinkyPulseMask", pinkyNorthwestMaskTex);
        bossMaterial.SetTexture("_ClydePulseMask", clydeNorthwestMaskTex);
    }
    public void SetEastMask()
    {
        bossMaterial.SetTexture("_BlinkyPulseMask", blinkyEastMaskTex);
        bossMaterial.SetTexture("_InkyPulseMask", inkyEastMaskTex);
        bossMaterial.SetTexture("_PinkyPulseMask", pinkyEastMaskTex);
        bossMaterial.SetTexture("_ClydePulseMask", clydeEastMaskTex);
    }
    public void SetWestMask()
    {
        bossMaterial.SetTexture("_BlinkyPulseMask", blinkyWestMaskTex);
        bossMaterial.SetTexture("_InkyPulseMask", inkyWestMaskTex);
        bossMaterial.SetTexture("_PinkyPulseMask", pinkyWestMaskTex);
        bossMaterial.SetTexture("_ClydePulseMask", clydeWestMaskTex);
    }
    public void SetAttackMask()
    {
        bossMaterial.SetTexture("_BlinkyPulseMask", blinkyAttackMaskTex);
        bossMaterial.SetTexture("_InkyPulseMask", inkyAttackMaskTex);
        bossMaterial.SetTexture("_PinkyPulseMask", pinkyAttackMaskTex);
        bossMaterial.SetTexture("_ClydePulseMask", clydeAttackMaskTex);
    }
    public void SetSlamMask()
    {
        bossMaterial.SetTexture("_BlinkyPulseMask", blinkySlamMaskTex);
        bossMaterial.SetTexture("_InkyPulseMask", inkySlamMaskTex);
        bossMaterial.SetTexture("_PinkyPulseMask", pinkySlamMaskTex);
        bossMaterial.SetTexture("_ClydePulseMask", clydeSlamMaskTex);
    }

    public void SetPulseWeight(int headID, int weight)
    {
        switch(headID)
        {
            case 0:
                bossMaterial.SetFloat("_InkyPulseWeight", weight);
                break;
            case 1:
                bossMaterial.SetFloat("_BlinkyPulseWeight", weight);
                break;
            case 2:
                bossMaterial.SetFloat("_PinkyPulseWeight", weight);
                break;
            case 3:
                bossMaterial.SetFloat("_ClydePulseWeight", weight);
                break;
        }
    }
}
