using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplaySliderValue : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text textBox;

    private void Start()
    {
        DisplayValue();
    }

    public void DisplayValue()
    {
        textBox.text = slider.value.ToString();
    }
}
