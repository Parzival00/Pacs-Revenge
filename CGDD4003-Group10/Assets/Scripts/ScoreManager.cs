using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private List<ScoreManagerAsset> highScores = new List<ScoreManagerAsset>();
    StreamReader savedScores;
    StreamWriter editScores;
    private int tempListIndex;

    private string playerIntials;

    [Header("UI Components")]
    [SerializeField] TMP_InputField uiInput;
    [SerializeField] TMP_Text highScoreDisplay;
    [SerializeField] TMP_Text currentPlayerScore;

    public struct ScoreManagerAsset
    {
        private int playerRank, playerScore;
        private string name;

        public ScoreManagerAsset(int rank, string intials, int score)
        {
            playerRank = rank;
            name = intials;
            playerScore = score;
        }
        public override string ToString()
        {
            return this.playerRank + " " + this.name + " " + this.playerScore + "\n";
        }
    }
    public void Start()
    {
        DisplayCurrentScore();
    }



    public void StoreScores()
    {
        string tempLine = "";
        string[] lineSplit;
        int tempRank, tempScore;
        ScoreManagerAsset tempScoreManager;

        tempListIndex = 10;

        if(!File.Exists((Application.persistentDataPath + "/Scores.txt"))){
            File.WriteAllText(Application.persistentDataPath + "/Scores.txt", "");
        }

        using (savedScores = new StreamReader(Application.persistentDataPath + "/Scores.txt"))
        {
            while (!savedScores.EndOfStream)
            {
                int counter = 0;
                tempLine = savedScores.ReadLine();
                lineSplit = tempLine.Split(' ');

                tempRank = Int32.Parse(lineSplit[0]);
                tempScore = Int32.Parse(lineSplit[2]);

                tempScoreManager = new ScoreManagerAsset(tempRank, lineSplit[1], tempScore);
                highScores.Add(tempScoreManager);

                if (Score.score >= tempScore && counter == 0)
                {
                    tempListIndex = tempRank;
                    counter += 1;
                }

                /*tempLine = savedScores.ReadLine();
                lineSplit = tempLine.Split(' ');

                tempRank = Int32.Parse(lineSplit[0]);
                tempScore = Int32.Parse(lineSplit[2]);
                tempScoreManager = new ScoreManagerAsset(tempRank, lineSplit[1], tempScore);
                highScores.Add(tempScoreManager);*/
            }
        }
    }
    public void AddPlayerScore()
    {
        StoreScores();

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

        if (tempListIndex <= 10)
        {
            highScores.Insert(tempListIndex, new ScoreManagerAsset(tempListIndex, playerIntials, Score.score));
        } else
        {
            highScores.Add(new ScoreManagerAsset(tempListIndex, playerIntials, Score.score));
        }

        if (highScores.Count > 10)
            highScores.RemoveAt(10);

        //highScores.Add(new ScoreManagerAsset(11,playerIntials,Score.score));

        //SortHighScores();

        DisplayHighScores();
    }
    /*public void SortHighScores() 
    {
        highScores.Sort();
        highScores.RemoveAt(11);
    }*/
    public void DisplayHighScores()
    {
        foreach (ScoreManagerAsset highscores in highScores)
        {
            highScoreDisplay.text += highscores.ToString();
        }
        WriteToScoreFile();
    }

    public void WriteToScoreFile() 
    {
        if (!File.Exists((Application.persistentDataPath + "/Scores.txt")))
        {
            File.WriteAllText(Application.persistentDataPath + "/Scores.txt", "");
        }
        using (editScores = new StreamWriter(Application.persistentDataPath + "/Scores.txt"))
        {
            foreach (ScoreManagerAsset highscores in highScores) 
            {
                editScores.WriteLine(highscores.ToString());
            }
        }
    }
    public void DisplayCurrentScore() 
    {
        int playerScore = Score.score;
        currentPlayerScore.text = "Current Score: " + playerScore;
    }




}
