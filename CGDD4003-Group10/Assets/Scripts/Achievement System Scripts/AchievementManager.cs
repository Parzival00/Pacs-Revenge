using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
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
[System.Serializable]
public class sEndings
{
    public bool ending0;
    public bool ending1;
    public bool ending2;
    public sEndings(bool e0, bool e1, bool e2)
    {
        this.ending0=e0;
        this.ending1=e1;
        this.ending2=e2;
    }
}
public class AchievementManager
{
    static string saveFile;
    static string fruitFile;
    static List<Achievement> potential = new List<Achievement>();
    static List<Achievement> collected = new List<Achievement>();

    static sEndings endings;
    static sInt deaths;
    static List<sInt> fruitCollected = new List<sInt>();

    static int deathsRequired = 100;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Load()
    {
        saveFile = Application.persistentDataPath + "/achievements.json";
        fruitFile = Application.persistentDataPath + "/fruits.json";
        //Debug.Log(saveFile);
        //populates every achievement to the potential list if there is no file found
        if (!File.Exists(saveFile))
        {
            endings = new sEndings(false, false, false);
            deaths = new sInt(0);
            potential.Add(new Achievement("AchievementImages/triple_threat", "Triple Threat", "triple_threat", "Get all three endings", false));
            potential.Add(new Achievement("AchievementImages/victory", "You Made It!", "victory", "Beat the boss and win the game", false));
            potential.Add(new Achievement("AchievementImages/corrupted", "Corruption", "corrupted", "Sucumb to the corruption and slaughter them all", false));
            potential.Add(new Achievement("AchievementImages/slow", "Too Slow!", "slow", "Fail to beat the boss in time", false));
            potential.Add(new Achievement("AchievementImages/baby", "Wah Wah!", "baby", "Play on Baby mode", false));
            potential.Add(new Achievement("AchievementImages/oof", "OOF", "oof", "Die 100 times", false));
            potential.Add(new Achievement("AchievementImages/dead_baby", "Seriously??", "dead_baby", "Die on baby mode", false));
            potential.Add(new Achievement("AchievementImages/massacre", "Massacre", "massacre", "Kill 15 ghosts on one level", false));
            potential.Add(new Achievement("AchievementImages/nom", "Nom Nom Nom", "nom", "Collect every kind of fruit", false));
            potential.Add(new Achievement("AchievementImages/speakers", "Where's That Coming From?", "speakers", "Check out the Boss' sound system", false));
            potential.Add(new Achievement("AchievementImages/speed", "Speedrunner", "speed", "Beat the boss with 2:30 or more left on the clock", false));
            potential.Add(new Achievement("AchievementImages/completed", "Completionist", "completed", "Get all achievements", false));
        }
        else
        {
            string[] jsonLines = File.ReadAllLines(saveFile);
            endings = JsonUtility.FromJson<sEndings>(jsonLines[0]);
            Debug.Log(endings.ending0);
            deaths = JsonUtility.FromJson<sInt>(jsonLines[1]);
            //Debug.Log(deaths.value);

            if (!File.Exists(fruitFile))
            {
                fruitCollected = new List<sInt>();
            }
            else
            {
                string[] fruitLines = File.ReadAllLines(fruitFile);
                foreach (string line in fruitLines)
                {
                    fruitCollected.Add(JsonUtility.FromJson<sInt>(line));
                }
            }

            for (int i = 2; i < jsonLines.Length; i++)
            {
                Achievement a = JsonUtility.FromJson<Achievement>(jsonLines[i]);
                if (a.collected)
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
        List<string> jsonLines = new List<string>
        {
            JsonUtility.ToJson(endings),
            JsonUtility.ToJson(deaths)
        };
        foreach (Achievement a in potential)
        {
            jsonLines.Add(JsonUtility.ToJson(a));
        }
        foreach (Achievement a in collected)
        {
            jsonLines.Add(JsonUtility.ToJson(a));
        }
        File.WriteAllLines(saveFile, jsonLines);

        jsonLines = new List<string>();
        foreach(sInt f in fruitCollected)
        {
            jsonLines.Add(JsonUtility.ToJson(f));
        }
        File.WriteAllLines(fruitFile, jsonLines);
    }

    /// <summary>
    /// This method takes the title of an achievement. It then displays the achievement to the player and adds it to the collected list.
    /// </summary>
    /// <param name="title"></param>
    public static void displayAchievement(string title)
    {
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
        else if (!current.collected)
        {
            Debug.Log($"Displaying {title} Achievement");

            GameObject popup = Resources.Load<GameObject>("AchievementBG");
            GameObject popupParent = GameObject.FindGameObjectWithTag("AchievementParent");
            if (popupParent == null)
            {
                popupParent = GameObject.FindFirstObjectByType<Canvas>().gameObject;
            }
            GameObject spawnedPopup = GameObject.Instantiate(popup, popupParent.transform);
            TextMeshProUGUI titleText = spawnedPopup.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI descripText = spawnedPopup.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            Image icon = spawnedPopup.transform.GetChild(0).GetComponentInChildren<Image>();

            current.collected = true;
            collected.Add(current);
            potential.Remove(current);
            titleText.text = current.title;
            descripText.text = current.description;
            icon.sprite = Resources.Load<Sprite>(current.imagePath);
            save(); //saves the new achievement to the save file

            for (int i = 0; i < collected.Count; i++)
            {
                Debug.Log($"Collected: {collected[i].title}");
            }
            for (int i = 0; i < potential.Count; i++) 
            {
                Debug.Log($"Potential: {potential[i].title}");
            }

            SteamIntegrationManager SIM = GameObject.FindObjectOfType<SteamIntegrationManager>();
            SIM.UnlockAchievement(current.api_name);

            //Completionist Achievement (Get all other achievements)
            SIM.checkCompletion();
            if(potential.Count == 1 && potential[0].title == "Completionist")
            {
                displayAchievement("Completionist");
            }
        }
    }

    public static void addDeath()
    {
        deaths.value = (deaths.value + 1);
        SteamIntegrationManager SIM = GameObject.FindObjectOfType<SteamIntegrationManager>();
        SIM.addDeath();

        //Seriously?? achievement (Die on baby mode)
        if (Score.difficulty == 0)
        {
            displayAchievement("Seriously??");
        }
        if(deaths.value >= 100)
        {
            displayAchievement("OOF");
        }

        save();
    }

    public static void addFruit(int f)
    {
        sInt sf = new sInt(f);
        if (!fruitObtained(sf))
        {
            fruitCollected.Add(sf);
            //Debug.Log(sf.value);
            //foreach (var item in fruitCollected)
            //    Debug.Log(item.value);
            SteamIntegrationManager SIM = GameObject.FindObjectOfType<SteamIntegrationManager>();
            SIM.addFruit();

            if (fruitCollected.Count >= 8)
            {
                displayAchievement("Nom Nom Nom");
            }
        }
        save();
    }

    public static void addEnding(int endingNum)
    {
        switch(endingNum)
        {
            case 0:
                if(!endings.ending0)
                {
                    endings.ending0 = true;
                    SteamIntegrationManager SIM = GameObject.FindObjectOfType<SteamIntegrationManager>();
                    SIM.addEnding();
                }
                break;
            case 1:
                if (!endings.ending1)
                {
                    endings.ending1 = true;
                    SteamIntegrationManager SIM = GameObject.FindObjectOfType<SteamIntegrationManager>();
                    SIM.addEnding();
                }
                break;
            case 2:
                if (!endings.ending2)
                {
                    endings.ending2 = true;
                    SteamIntegrationManager SIM = GameObject.FindObjectOfType<SteamIntegrationManager>();
                    SIM.addEnding();
                }
                break;

        }

        checkEndings();
        save();
    }

    public static void checkEndings()
    {
        if(endings.ending0 && endings.ending1 && endings.ending2)
        {
            displayAchievement("Triple Threat");
        }
    }

    private static bool fruitObtained(sInt f)
    {
        foreach (sInt item in fruitCollected)
        {
            if(item.value == f.value)
            {
                return true;
            }
        }
        return false;
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
