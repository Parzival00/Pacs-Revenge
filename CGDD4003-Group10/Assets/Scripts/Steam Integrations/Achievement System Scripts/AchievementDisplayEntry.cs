using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AchievementDisplayEntry : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text description;
    [SerializeField] CanvasGroup canvasGroup;

   public void Display(Achievement achievement)
    {
        icon.sprite = Resources.Load<Sprite>(achievement.imagePath);
        Localizer.TextIdentifier descriptionIdentifier = (Localizer.TextIdentifier)Enum.Parse(typeof(Localizer.TextIdentifier), $"Achievement_Description_{achievement.api_name}");
        title.text = achievement.title;
        description.font = Localizer.instance.GetCurrentFont();
        description.text = Localizer.instance.GetLanguageText(descriptionIdentifier);

        canvasGroup.alpha = achievement.collected ? 1f : 0.3f;
    }
}
