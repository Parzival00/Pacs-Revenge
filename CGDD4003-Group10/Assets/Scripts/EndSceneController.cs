using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndSceneController : MonoBehaviour
{
    [System.Serializable]
    public struct EndText
    {
        public string[] paragraphs;
    }

    [Header("End Cutscene")]
    [SerializeField] GameObject skipButton;
    [SerializeField] TMP_Text textBox;
    [SerializeField] float startDelay = 1.5f;
    [SerializeField] float writingRate = 10;
    [SerializeField] float timeTillCanSkip = 3f;
    [SerializeField] EndText ending0Text;
    [SerializeField] EndText ending1Text;
    [SerializeField] EndText ending2Text;

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

        EndText endingText = new EndText();
        int ending = PlayerPrefs.GetInt("Ending", 1);
        switch(ending)
        {
            case 0:
                endingText = ending0Text;
                break;
            case 1:
                endingText = ending1Text;
                break;
            case 2:
                endingText = ending2Text;
                break;
        }

        float timer = 0;

        textBox.text = "";
        for (int i = 0; i < endingText.paragraphs.Length; i++)
        {
            for (int e = 0; e < endingText.paragraphs[i].Length; e++)
            {
                if (timer >= timeTillCanSkip) skipButton.SetActive(true);

                textBox.text += endingText.paragraphs[i][e];
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
