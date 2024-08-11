using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementDisplayEntry : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text description;
    [SerializeField] CanvasGroup canvasGroup;

   public void Display(Achievement achievement)
    {
        icon.sprite = Resources.Load<Sprite>(achievement.imagePath);
        title.text = achievement.title;
        description.text = achievement.description;

        canvasGroup.alpha = achievement.collected ? 1f : 0.3f;
    }
}
