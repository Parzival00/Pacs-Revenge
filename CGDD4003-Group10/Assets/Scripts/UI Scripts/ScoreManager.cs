using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private List<ScoreEntry> highScores = new List<ScoreEntry>();
    StreamReader savedScores;
    StreamWriter editScores;
    private int tempListIndex;

    private string playerIntials;

    [SerializeField] bool deleteHighScoreFile;

    [Header("UI Components")]
    [SerializeField] Camera cam;
    [SerializeField] TMP_InputField uiInput;
    [SerializeField] TMP_Text highScoreDisplay;
    [SerializeField] TMP_Text currentPlayerScore;
    [SerializeField] TMP_Text babyModeInsult;
    [SerializeField] Image caret;
    [SerializeField] float characterWidth;
    [SerializeField] float blinkRate = 3f;
    Vector2 caretStartPos;

    RectTransform caretRect;

    bool wroteToFile = false;

    public struct ScoreEntry
    {
        public int playerRank, playerScore;
        public string name;

        public ScoreEntry(int rank, string intials, int score)
        {
            playerRank = rank;
            name = intials;
            playerScore = score;
        }
        public override string ToString()
        {
            return string.Format("{0,-4}{1,4}", $"{this.playerRank}.", this.name) + string.Format("{0,10}", this.playerScore);// this.playerRank + ".   " + this.name + "   " + this.playerScore;
        }
        public string ToFileFormat()
        {
            return this.playerRank + " " + this.name + " " + this.playerScore;
        }
    }

    public void Start()
    {
        wroteToFile = false;

        if (deleteHighScoreFile)
        {
            if (File.Exists((Application.persistentDataPath + "/Scores.txt")))
            {
                File.Delete(Application.persistentDataPath + "/Scores.txt");
            }
        }

        if(PlayerPrefs.GetInt("Difficulty") == 0) 
        {
            uiInput.gameObject.SetActive(false);
            highScoreDisplay.gameObject.SetActive(false);
            currentPlayerScore.gameObject.SetActive(false);
            babyModeInsult.gameObject.SetActive(true); 
        } 
        else
        {
            babyModeInsult.gameObject.SetActive(false);
            uiInput.gameObject.SetActive(true);
            highScoreDisplay.gameObject.SetActive(true);
            currentPlayerScore.gameObject.SetActive(true);
            DisplayCurrentScore();
        }

        caretRect = caret.GetComponent<RectTransform>();
        caretStartPos = caretRect.anchoredPosition;
    }


    public void ReadScores()
    {
        string tempLine = "";
        string[] lineSplit;
        int tempRank, tempScore;
        ScoreEntry tempScoreManager;

        if(!File.Exists((Application.persistentDataPath + "/Scores.txt"))){
            File.WriteAllText(Application.persistentDataPath + "/Scores.txt", "");
        }

        using (savedScores = new StreamReader(Application.persistentDataPath + "/Scores.txt"))
        {
            while (!savedScores.EndOfStream)
            {
                tempLine = savedScores.ReadLine();
                lineSplit = tempLine.Split(' ');

                if (lineSplit.Length != 3) continue;

                tempRank = Int32.Parse(lineSplit[0]);
                tempScore = Int32.Parse(lineSplit[2]);

                tempScoreManager = new ScoreEntry(tempRank, lineSplit[1], tempScore);
                highScores.Add(tempScoreManager);
            }
        }
    }
    public void AddPlayerScore(string input)
    {
        ReadScores();

        //if (Input.GetKeyDown(KeyCode.Return))
        //{
            if (uiInput.text.Equals("") || uiInput.Equals(null))
            {
                playerIntials = "BOO";
            }
            playerIntials = uiInput.text.ToUpper();
            uiInput.gameObject.SetActive(false);
            currentPlayerScore.gameObject.SetActive(false);
        //}

        bool matchingNameFound = false;
        bool removedMatchingName = false;

        //Check for matching name already in high score list and remove it
        for (int i = 0; i < highScores.Count; i++)
        {
            if(highScores[i].name == playerIntials)
            {
                matchingNameFound = true;
                if (highScores[i].playerScore < Score.score)
                {
                    removedMatchingName = true;
                    print("Matching Name");
                    highScores.RemoveAt(i);

                    //Update rank positions of all entries after removed entry
                    for (int e = i; e < highScores.Count; e++)
                    {
                        ScoreEntry temp = highScores[e];
                        temp.playerRank -= 1;

                        highScores.Insert(e, temp);
                        highScores.RemoveAt(e + 1);
                    }
                }
            }
        }

        if (!matchingNameFound || (matchingNameFound && removedMatchingName))
        {
            bool foundInsertLocation = false;
            tempListIndex = highScores.Count;

            //Find index of new entry into high score list
            for (int i = 0; i < highScores.Count; i++)
            {
                if (Score.score > highScores[i].playerScore && foundInsertLocation == false)
                {
                    foundInsertLocation = true;

                    tempListIndex = i;
                }
            }

            //Add new entry
            if (tempListIndex < highScores.Count)
            {
                highScores.Insert(tempListIndex, new ScoreEntry(tempListIndex + 1, playerIntials, Score.score));
            }
            else
            {
                highScores.Add(new ScoreEntry(highScores.Count + 1, playerIntials, Score.score));
            }

            //Trim list to only ten entries
            //if (highScores.Count > 10)
            //    highScores.RemoveAt(10);

            //Update rank positions of all entries after newly inserted entry
            for (int i = tempListIndex + 1; i < highScores.Count; i++)
            {
                ScoreEntry temp = highScores[i];
                temp.playerRank += 1;

                highScores.Insert(i, temp);
                highScores.RemoveAt(i + 1);
            }
        }

        DisplayHighScores();
    }

    public void DisplayHighScores()
    {
        highScoreDisplay.text = "";
        for(int i = 0; i < 10 && i < highScores.Count; i++)
        {
            highScoreDisplay.text += highScores[i].ToString() + "\n";
        }

        if(!wroteToFile)
            WriteToScoreFile();
    }

    public void WriteToScoreFile() 
    {
        wroteToFile = true;

        if (!File.Exists((Application.persistentDataPath + "/Scores.txt")))
        {
            File.WriteAllText(Application.persistentDataPath + "/Scores.txt", "");
        }

        using (editScores = new StreamWriter(Application.persistentDataPath + "/Scores.txt"))
        {
            foreach (ScoreEntry highscores in highScores) 
            {
                editScores.WriteLine(highscores.ToFileFormat());
            }
        }
    }
    public void DisplayCurrentScore() 
    {
        int playerScore = Score.score;
        currentPlayerScore.text = "Score: " + playerScore;
    }

    Coroutine blinkCoroutine;
    IEnumerator BlinkCaret()
    {
        float timeBtwBlinks = 1 / blinkRate;
        float timer = 0;
        while(caret.gameObject.activeInHierarchy)
        {
            if(timer > timeBtwBlinks)
            {
                caret.enabled = !caret.enabled;
                timer = 0;
            }
            yield return null;
            timer += Time.deltaTime;
        }
    }

    public void OnValueChangedInput()
    {
        string s = uiInput.text.ToUpper();

        caretRect.anchoredPosition = caretStartPos + Vector2.right * s.Length * characterWidth;

        if (s.Length >= 3)
        {
            caret.gameObject.SetActive(false);
        }
    }

    public void OnSelectInput()
    {
        if (uiInput.text.ToUpper().Length < 3)
        {
            caret.gameObject.SetActive(true);
            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
            blinkCoroutine = StartCoroutine(BlinkCaret());
        }
    }

    public void OnDeselectInput()
    {
        caret.gameObject.SetActive(false);
    }
}
