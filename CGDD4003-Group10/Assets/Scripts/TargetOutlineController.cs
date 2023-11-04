using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetOutlineController : MonoBehaviour
{
    [SerializeField] Sprite outlineTarget;
    [SerializeField] float timeBtwBlinks;
    [SerializeField] Color targetDifficultyEasy = Color.yellow;
    [SerializeField] Color targetDifficultyMedium;
    [SerializeField] Color targetDifficultyHard = Color.red;

    public Color TargetDifficultyEasy { get => targetDifficultyEasy; }
    public Color TargetDifficultyMedium { get => targetDifficultyMedium; }
    public Color TargetDifficultyHard { get => targetDifficultyHard; }

    TargetAreaCollider.TargetInfo currentTarget;

    public Color tempColorSave;

    //Coroutine activeCoroutine;

    public void SetTargetOutline(TargetAreaCollider.TargetInfo target)
    {
        if (currentTarget.outline != target.outline)
            DeactivateOutline();

        if (target.outline != null && target.outline != currentTarget.outline)
        {
            currentTarget = target;

            currentTarget.outline.enabled = true;
            currentTarget.outline.sprite = outlineTarget;

            switch (target.areaDifficulty)
            {
                case Ghost.TargetAreaDifficulty.Easy:
                    currentTarget.outline.color = targetDifficultyEasy;
                    tempColorSave = currentTarget.outline.color;
                    break;
                case Ghost.TargetAreaDifficulty.Medium:
                    currentTarget.outline.color = targetDifficultyMedium;
                    tempColorSave = currentTarget.outline.color;
                    break;
                case Ghost.TargetAreaDifficulty.Hard:
                    currentTarget.outline.color = targetDifficultyHard;
                    tempColorSave = currentTarget.outline.color;
                    break;
            }
            ///if (activeCoroutine == null)
             //   activeCoroutine = StartCoroutine(Blink());
        }
    }
    public Color GetOutlineColor(TargetAreaCollider.TargetInfo target) 
    {
        return tempColorSave;
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
        //if(activeCoroutine != null)
        //    StopCoroutine(activeCoroutine);

        if (currentTarget.outline != null)
            currentTarget.outline.enabled = false;

        currentTarget.outline = null;
        //activeCoroutine = null;
    }
}
