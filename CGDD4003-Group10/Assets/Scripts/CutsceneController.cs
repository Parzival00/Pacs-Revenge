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
        public Localizer.TextIdentifier[] paragraphs;
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
    [SerializeField] GameObject[] creditPages;
    [SerializeField] GameObject continueButton;
    [SerializeField] float creditsShowLength = 5f;

    [Header("Audio Sources")]
    [SerializeField] AudioSource victoryMusic;
    [SerializeField] AudioSource creditsMusic;

    bool skipped = false;

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
            //ending = 2;
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

        if (victoryMusic != null)
        {
            victoryMusic.Play();
        }

        float timer = 0;

        textBox.text = "";
        textBox.font = Localizer.instance.GetCurrentFont();
        for (int i = 0; i < cutsceneText.paragraphs.Length; i++)
        {
            string paragraphText = Localizer.instance.GetLanguageText(cutsceneText.paragraphs[i]);
            for (int e = 0; e < paragraphText.Length; e++)
            {
                if (skipped) break;

                if (timer >= timeTillCanSkip) skipButton.SetActive(true);

                textBox.text += paragraphText[e];
                yield return writingWait;
                timer += 1f / writingRate;
            }
            textBox.text += "\n\n";

            if (skipped) break;
        }

        skipButton.SetActive(!skipped);
    }

    public void ShowCredits()
    {
        StartCoroutine(ShowCreditsSequence());
    }
    IEnumerator ShowCreditsSequence()
    {
        skipped = true;

        StartCoroutine(PlayCreditsMusic());

        textBox.gameObject.SetActive(false);

        credits.gameObject.SetActive(true);

        continueButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);

        for (int i = 0; i < creditPages.Length; i++)
        {
            

            creditPages[i].gameObject.SetActive(true);
            if (i == creditPages.Length - 1)
            {
                continueButton.gameObject.SetActive(true);
                break;
            }
            yield return new WaitForSeconds(creditsShowLength);
            creditPages[i].gameObject.SetActive(false);
        }
    }

    IEnumerator PlayCreditsMusic()
    {
        if (victoryMusic != null && victoryMusic.isPlaying) 
        {
            float startVolume = victoryMusic.volume;

            float t = 0;
            float deafenLength = 1f;
            while (t < deafenLength)
            {
                victoryMusic.volume = Mathf.Lerp(0, startVolume, 1 - t / deafenLength);
                yield return null;
                t += Time.deltaTime;
            }
            victoryMusic.volume = 0;
            victoryMusic.Stop();
        }

        if (creditsMusic != null)
        {
            creditsMusic.Play();
        }
    }
}
