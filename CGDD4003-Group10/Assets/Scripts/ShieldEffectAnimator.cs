using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEffectAnimator : MonoBehaviour
{
    [SerializeField] Material shieldEffect;

    bool isAnimating = false;

    // Start is called before the first frame update
    void Start()
    {
        shieldEffect.SetInt("_Active", 0);
        isAnimating = false;
    }

    public void PlayShieldUp()
    {
        shieldEffect.SetInt("_Active", 1);

        StartCoroutine(ShieldUpAnimation());
    }

    IEnumerator ShieldUpAnimation()
    {
        yield return new WaitUntil(() => isAnimating == false);
    }

    public void PlaySheildDown()
    {
        shieldEffect.SetInt("_Active", 0);

        StartCoroutine(ShieldDownAnimation());
    }

    IEnumerator ShieldDownAnimation()
    {
        yield return new WaitUntil(() => isAnimating == false);
    }

    private void OnApplicationQuit()
    {
        shieldEffect.SetInt("_Active", 0);
    }
}
