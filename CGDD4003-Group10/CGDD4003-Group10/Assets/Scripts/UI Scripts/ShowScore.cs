using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowScore : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    private void Awake()
    {
        print(Score.score);
        scoreText.text = "Score: " + Score.score;
    }
}
