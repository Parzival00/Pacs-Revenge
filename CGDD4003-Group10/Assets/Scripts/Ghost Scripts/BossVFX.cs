using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossVFX : MonoBehaviour
{
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

    // Start is called before the first frame update
    void Start()
    {
        if (auraSpriteRenderer == null || enrageParticleSystem == null) Debug.LogError("Boss VFX not properly set. Check Inspector");
        auraMaterial = auraSpriteRenderer.material;
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
}
