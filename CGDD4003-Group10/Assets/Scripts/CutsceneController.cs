using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CutsceneController : MonoBehaviour
{
    [System.Serializable]
    public struct CutsceneText
    {
        public string[] paragraphs;
    }

    [Header("Cutscenes")]
    [SerializeField] GameObject skipButton;
    [SerializeField] TMP_Text textBox;
    [SerializeField] float startDelay = 1.5f;
    [SerializeField] float writingRate = 10;
    [SerializeField] float timeTillCanSkip = 3f;
    [SerializeField] CutsceneText ending0Text;
    [SerializeField] CutsceneText ending1Text;
    [SerializeField] CutsceneText ending2Text;
    [SerializeField] CutsceneText beginningText;

    [Header("Credits")]
    [SerializeField] GameObject credits;
    [SerializeField] GameObject continueButton;
    [SerializeField] float creditsShowLength = 5f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PrintText());
    }

    IEnumerator PrintText()
    {
        yield return new WaitForSeconds(startDelay);

        WaitForSeconds writingWait = new WaitForSeconds(1f / writingRate);

        CutsceneText cutsceneText = new CutsceneText();
        if (SceneManager.GetActiveScene().name == "Start")
        {
            cutsceneText = beginningText;
        }
        else
        {
            int ending = PlayerPrefs.GetInt("Ending", 1);
            switch (ending)
            {
                case 0:
                    cutsceneText = ending0Text;
                    break;
                case 1:
                    cutsceneText = ending1Text;
                    break;
                case 2:
                    cutsceneText = ending2Text;
                    break;
            }
        }

        float timer = 0;

        textBox.text = "";
        for (int i = 0; i < cutsceneText.paragraphs.Length; i++)
        {
            for (int e = 0; e < cutsceneText.paragraphs[i].Length; e++)
            {
                if (timer >= timeTillCanSkip) skipButton.SetActive(true);

                textBox.text += cutsceneText.paragraphs[i][e];
                yield return writingWait;
                timer += 1f / writingRate;
            }
            textBox.text += "\n\n";
        }

        skipButton.SetActive(true);
    }

    public void ShowCredits()
    {
        StartCoroutine(ShowCreditsSequence());
    }
    IEnumerator ShowCreditsSequence()
    {
        credits.gameObject.SetActive(true);
        textBox.gameObject.SetActive(false);

        continueButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(creditsShowLength);

        continueButton.gameObject.SetActive(true);
    }
}
