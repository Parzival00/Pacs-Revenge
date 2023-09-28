using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int playerRank, playerScore;
    private string name;

    private List<ScoreManager> highScores;
    StreamReader savedScores;

    private string playerIntials;

    [Header("UI Components")]
    [SerializeField] TMP_InputField uiInput;
    [SerializeField] TMP_Text highScoreDisplay;
    public ScoreManager(int rank, string intials, int score) 
    {
        playerRank = rank;
        name = intials;
        playerScore = score;
    }
    public override string ToString()
    {
        return this.playerRank + " " + this.name + " " + this.playerScore + "\n";
    }

    public void StoreScores() 
    {
        string tempLine = "";
        string[] lineSplit;
        int tempRank, tempScore;
        ScoreManager tempScoreManager;

        using(savedScores = new StreamReader(Application.dataPath + "/Scores.txt")) 
        {
            while (savedScores.ReadLine() != null) 
            {
                tempLine = savedScores.ReadLine();
                lineSplit = tempLine.Split(' ');

                tempRank = Int32.Parse(lineSplit[0]);
                tempScore = Int32.Parse(lineSplit[2]);
                tempScoreManager = new ScoreManager(tempRank, lineSplit[1], tempScore);
                highScores.Add(tempScoreManager);
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
        }

        highScores.Add(new ScoreManager(11,playerIntials,Score.score));

        SortHighScores();

        DisplayHighScores();
    }
    public void SortHighScores() 
    {
        highScores.Sort();
        highScores.RemoveAt(11);
    }
    public void DisplayHighScores() 
    {
        foreach (ScoreManager highscores in highScores) 
        {
            highScoreDisplay.text = highscores.ToString();
        }
    }

    

    
}
