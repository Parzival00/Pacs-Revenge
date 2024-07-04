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
        Debug.Log(saveFile);
        //populates every achievement to the potential list if there is no file found
        if(!File.Exists(saveFile))
        {
            endings = new sInt(0);
            deaths = new sInt(0);
            fruitCollected = new sInt(0);
            potential.Add(new Achievement("AchievementImages/triple_threat.png", "Triple Threat", "Get all three endings", false));
            potential.Add(new Achievement("AchievementImages/victory.png", "You Made It!", "Beat the boss and win the game", false));
            potential.Add(new Achievement("AchievementImages/corrupted.png", "Corruption", "Sucumb to the corruption and slaughter them all", false));
            potential.Add(new Achievement("AchievementImages/slow.png", "Too Slow!", "Fail to beat the boss in time", false));
            potential.Add(new Achievement("AchievementImages/baby.png", "Wah Wah!", "Play on Baby mode", false));
            potential.Add(new Achievement("AchievementImages/oof.png", "OOF", "Die x amount of times", false));
            potential.Add(new Achievement("/AchievementImages/dead_baby.png", "Seriously??", "Die on baby mode", false));
            potential.Add(new Achievement("AchievementImages/massacre.png", "Massacre", "Kill x ghosts on one level", false));
            potential.Add(new Achievement("AchievementImages/nom.png", "Nom Nom Nom", "Collect every kind of fruit", false));
            potential.Add(new Achievement("AchievementImages/speakers.png", "Where's That Coming From?", "Check out the Boss' sound system", false));
            potential.Add(new Achievement("AchievementImages/speed.png", "Speed Racer", "Beat the boss in x amount of time", false));
        }
        else
        {
            string[] jsonLines = File.ReadAllLines(saveFile);
            endings = JsonUtility.FromJson<sInt>(jsonLines[0]);
            Debug.Log(endings.value);
            deaths = JsonUtility.FromJson<sInt>(jsonLines[1]);
            Debug.Log(deaths.value);
            fruitCollected = JsonUtility.FromJson<sInt>(jsonLines[2]);
            Debug.Log(fruitCollected.value);
            for (int i = 3; i < jsonLines.Length; i++)
            {
                Achievement a = JsonUtility.FromJson<Achievement>(jsonLines[i]);
                if(a.collected)
                {
                    Debug.Log(a.title + " was collected");
                    collected.Add(a);
                }
                else
                {
                    Debug.Log(a.title + " was not collected");
                    potential.Add(a);
                }
            }
        }
    }
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
            titleText.text = current.title;
            descripText.text = current.description;
            icon.sprite = Resources.Load<Sprite>(current.imagePath);
        }
    }
}
