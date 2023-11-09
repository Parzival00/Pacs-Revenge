using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private List<ScoreEntry> highScores = new List<ScoreEntry>();
    StreamReader savedScores;
    StreamWriter editScores;
    private int tempListIndex;

    private string playerIntials;

    [SerializeField] bool deleteHighScoreFile;

    [Header("UI Components")]
    [SerializeField] TMP_InputField uiInput;
    [SerializeField] TMP_Text highScoreDisplay;
    [SerializeField] TMP_Text currentPlayerScore;
    [SerializeField] TMP_Text babyModeInsult;

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

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (uiInput.text.Equals("") || uiInput.Equals(null))
            {
                playerIntials = "BOO";
            }
            playerIntials = uiInput.text.ToUpper();
            uiInput.gameObject.SetActive(false);
            currentPlayerScore.gameObject.SetActive(false);
        }


        //Check for matching name already in high score list and remove it
        for (int i = 0; i < highScores.Count; i++)
        {
            if(highScores[i].name == playerIntials)
            {
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
        } else
        {
            highScores.Add(new ScoreEntry(highScores.Count + 1, playerIntials, Score.score));
        }

        //Trim list to only ten entries
        if (highScores.Count > 10)
            highScores.RemoveAt(10);

        //Update rank positions of all entries after newly inserted entry
        for (int i = tempListIndex + 1; i < highScores.Count; i++)
        {
            ScoreEntry temp = highScores[i];
            temp.playerRank += 1;

            highScores.Insert(i, temp);
            highScores.RemoveAt(i + 1);
        }

        DisplayHighScores();
    }

    public void DisplayHighScores()
    {
        highScoreDisplay.text = "";
        foreach (ScoreEntry highscore in highScores)
        {
            highScoreDisplay.text += highscore.ToString() + "\n";
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
                editScores.WriteLine(highscores.ToString());
            }
        }
    }
    public void DisplayCurrentScore() 
    {
        int playerScore = Score.score;
        currentPlayerScore.text = "Score: " + playerScore;
    }
}
