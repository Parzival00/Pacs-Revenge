using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveData
{
    static string saveFile;

    static sInt currentLevelIndex;
    static List<string> unlockedWeapons = new List<string>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Load()
    {
        saveFile = Application.persistentDataPath + "/playerData.json";
        if (!File.Exists(saveFile))
        {
            Debug.Log("Could not find save file... using default values");
            currentLevelIndex.value = 0;
        }
        else
        {
            Debug.Log("Save file found...loading data");
            string[] jsonLines = File.ReadAllLines(saveFile);
            currentLevelIndex = JsonUtility.FromJson<sInt>(jsonLines[0]);

            //Add Weapons to list of unlocked
            for(int i = 1; i < jsonLines.Length; i++)
            {
                unlockedWeapons.Add(JsonUtility.FromJson<string>(jsonLines[i]));
            }
            Debug.Log("Current level: " + currentLevelIndex + "\nNumber of weapons Unlocked: " + unlockedWeapons.Count);
        }
    }

    public static void Save()
    {
        List<string> jsonLines = new List<string>();
        jsonLines.Add(JsonUtility.ToJson(currentLevelIndex));
        jsonLines.Add(JsonUtility.ToJson(unlockedWeapons));
    }

    public static void updateLevel(int level)
    {
        currentLevelIndex.value = level;
    }

    public static void addWeaponUnlock(string weaponName)
    {
        unlockedWeapons.Add(weaponName);
    }
}
