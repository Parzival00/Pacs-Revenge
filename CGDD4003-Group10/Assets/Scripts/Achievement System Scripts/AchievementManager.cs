using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
//Had to make my own serializable int type bc normal ints dont work for json
[System.Serializable]
public class sInt
{
    public int value; 
    public sInt(int i)
    {
        value = i;
    }
}
public class AchievementManager
{
    static string saveFile;
    static List<Achievement> potential = new List<Achievement>();
    static List<Achievement> collected = new List<Achievement>();

    static sInt endings;
    static sInt deaths;
    static sInt fruitCollected;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Load()
    {
        saveFile = Application.persistentDataPath + "/achievements.json";
        //Debug.Log(saveFile);
        //populates every achievement to the potential list if there is no file found
        if(!File.Exists(saveFile))
        {
            endings = new sInt(0);
            deaths = new sInt(0);
            fruitCollected = new sInt(0);
            potential.Add(new Achievement("AchievementImages/triple_threat", "Triple Threat", "Get all three endings", false));
            potential.Add(new Achievement("AchievementImages/victory", "You Made It!", "Beat the boss and win the game", false));
            potential.Add(new Achievement("AchievementImages/corrupted", "Corruption", "Sucumb to the corruption and slaughter them all", false));
            potential.Add(new Achievement("AchievementImages/slow", "Too Slow!", "Fail to beat the boss in time", false));
            potential.Add(new Achievement("AchievementImages/baby", "Wah Wah!", "Play on Baby mode", false));
            potential.Add(new Achievement("AchievementImages/oof", "OOF", "Die x amount of times", false));
            potential.Add(new Achievement("AchievementImages/dead_baby", "Seriously??", "Die on baby mode", false));
            potential.Add(new Achievement("AchievementImages/massacre", "Massacre", "Kill x ghosts on one level", false));
            potential.Add(new Achievement("AchievementImages/nom", "Nom Nom Nom", "Collect every kind of fruit", false));
            potential.Add(new Achievement("AchievementImages/speakers", "Where's That Coming From?", "Check out the Boss' sound system", false));
            potential.Add(new Achievement("AchievementImages/speed", "Speedrunner", "Beat the boss in x amount of time", false));
            potential.Add(new Achievement("AchievementImages/completed", "Completionist", "Get all achievements", false));
        }
        else
        {
            string[] jsonLines = File.ReadAllLines(saveFile);
            endings = JsonUtility.FromJson<sInt>(jsonLines[0]);
            //Debug.Log(endings.value);
            deaths = JsonUtility.FromJson<sInt>(jsonLines[1]);
            //Debug.Log(deaths.value);
            fruitCollected = JsonUtility.FromJson<sInt>(jsonLines[2]);
           // Debug.Log(fruitCollected.value);
            for (int i = 3; i < jsonLines.Length; i++)
            {
                Achievement a = JsonUtility.FromJson<Achievement>(jsonLines[i]);
                if(a.collected)
                {
                    //Debug.Log(a.title + " was collected");
                    collected.Add(a);
                }
                else
                {
                    //Debug.Log(a.title + " was not collected");
                    potential.Add(a);
                }
            }
        }
    }
    /// <summary>
    /// Saves the current values of endings,  deaths, fruit collected, and the achievement lists to a json file.
    /// </summary>
    public static void save()
    {
        List<string> jsonLines = new List<string>();
        jsonLines.Add(JsonUtility.ToJson(endings));
        jsonLines.Add(JsonUtility.ToJson(deaths));
        jsonLines.Add(JsonUtility.ToJson(fruitCollected));
        foreach (Achievement a in potential)
        {
            jsonLines.Add(JsonUtility.ToJson(a));
        }
        foreach (Achievement a in collected)
        {
            jsonLines.Add(JsonUtility.ToJson(a));
        }
        File.WriteAllLines(saveFile, jsonLines);
    }
    /// <summary>
    /// This method takes the title of an achievement. It then displays the achievement to the player and adds it to the collected list.
    /// </summary>
    /// <param name="title"></param>
    public static void displayAchievement(string title)
    {
        GameObject popup = Resources.Load<GameObject>("AchievementBG");
        GameObject.Instantiate(popup);
        TextMeshProUGUI titleText = GameObject.FindGameObjectWithTag("ATitle").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI descripText = GameObject.FindGameObjectWithTag("ADescription").GetComponent<TextMeshProUGUI>();
        Image icon = popup.GetComponentInChildren<Image>();

        Achievement current = null;
        foreach (Achievement a in potential)
        {
            if(a.title.Equals(title))
            {
                current = a;
                break;
            }
        }

        if(current == null)
        {
            Debug.Log("No Achievement Found");
        }
        else
        {
            collected.Add(current);
            potential.Remove(current);
            titleText.text = current.title;
            descripText.text = current.description;
            icon.sprite = Resources.Load<Sprite>(current.imagePath);
            save(); //saves the new achievement to the save file

            //Completionist Achievement (Get all other achievements)
            //Needs art to add the real achievement
            if (collected.Count <= 1)
                displayAchievement("Completionist");
        }
    }
    public static void addDeath()
    {
        deaths.value = (deaths.value + 1);

        //Oof achievement (Die a certain amount of times)
        int deathsRequired = 100;

        if (deaths.value >= deathsRequired)
            displayAchievement("OOF");

        //Seriously?? achievement (Die on baby mode)
        if (Score.difficulty == 0)
            displayAchievement("Seriously??");
            
        save();
    }

    public static void addFruit()
    {
        fruitCollected.value = (fruitCollected.value + 1);

        //Nom Nom Nom achievement (Eat a fruit)
        if (Score.difficulty == 0)
            displayAchievement("Nom Nom Nom");
        
        save();
    }

    public static List<Achievement> getPotentialAchievements()
    {
        return potential;
    }
    public static List<Achievement> getCompletedAchievements()
    {
        return collected;
    }
}
