using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocalizedText : MonoBehaviour
{
    [SerializeField] TMP_Text textField;
    [SerializeField] Localizer.TextIdentifier textIdentifier;

    bool subscribedSuccessfully = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!subscribedSuccessfully)
        {
            Localizer.instance.OnLanguageChanged += HandleLanguageChange;
        }
        HandleLanguageChange();
    }

    public void HandleLanguageChange()
    {
        if (textIdentifier != Localizer.TextIdentifier.None)
        {
            print("Set text to new language");
            textField.text = Localizer.instance.GetLanguageText(textIdentifier);
            textField.font = Localizer.instance.GetCurrentFont();
        }
    }

    private void OnEnable()
    {
        if (Localizer.instance != null)
        {
            Localizer.instance.OnLanguageChanged += HandleLanguageChange;
            subscribedSuccessfully = true;
            HandleLanguageChange();
        }
    }
    private void OnDisable()
    {
        if (Localizer.instance != null)
        {
            Localizer.instance.OnLanguageChanged -= HandleLanguageChange;
            subscribedSuccessfully = false;
        }
    }
}
