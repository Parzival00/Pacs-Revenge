using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetOutlineController : MonoBehaviour
{
    [SerializeField] Sprite outlineTarget;
    [SerializeField] float timeBtwBlinks;

    SpriteRenderer currentOutline;

    Coroutine activeCoroutine;

    public void SetTargetOutline(SpriteRenderer target)
    {
        if (currentOutline != target)
            DeactivateOutline();

        if (target != null && target != currentOutline)
        {
            currentOutline = target;

            currentOutline.enabled = true;
            currentOutline.sprite = outlineTarget;

            ///if (activeCoroutine == null)
             //   activeCoroutine = StartCoroutine(Blink());
        }
    }

   /* IEnumerator Blink()
    {
        WaitForSeconds wait = new WaitForSeconds(timeBtwBlinks);

        while (true)
        {
            currentOutline.sprite = outlineTarget;
            yield return wait;
            currentOutline.sprite = outlineTargetFill;
            yield return wait;
        }
    }*/

    public void DeactivateOutline()
    {
        if(activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        if (currentOutline != null)
            currentOutline.enabled = false;

        currentOutline = null;
        activeCoroutine = null;
    }
}
