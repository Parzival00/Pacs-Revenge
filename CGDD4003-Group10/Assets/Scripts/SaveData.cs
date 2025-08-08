using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public static class SaveData
{
    static string saveFile;

    static List<sInt> unlockedWeapons = new List<sInt>();
    static sInt currentLevelIndex;
    static sInt currentWeapon;
    static sInt currentDifficulty;

    static SaveData()
    {
        saveFile = saveFile = Application.persistentDataPath + "/playerData.json";

        currentLevelIndex = new sInt(0);
        currentWeapon = new sInt(0);
        currentDifficulty = new sInt(0);
        unlockedWeapons = new List<sInt>();
        unlockedWeapons.Add(new sInt(0));

        Debug.Log(saveFile);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Load()
    {
        if (!File.Exists(saveFile))
        {
            Debug.Log("Could not find save file... using default values");
            currentLevelIndex.value = 0;
        }
        else
        {
            Debug.Log("Save file found...loading data");
            string[] jsonLines = File.ReadAllLines(saveFile);
            int lineIndex = 0;
            Debug.Log("Found " + jsonLines.Length + " lines");

            currentLevelIndex = JsonUtility.FromJson<sInt>(jsonLines[lineIndex++]);
            currentWeapon = JsonUtility.FromJson<sInt>(jsonLines[lineIndex++]);
            currentDifficulty = JsonUtility.FromJson<sInt>(jsonLines[lineIndex++]);

            for(int i = lineIndex; i < jsonLines.Length; i++)
            {
                unlockedWeapons.Add(JsonUtility.FromJson<sInt>(jsonLines[i]));
            }


            //PlayerPrefs.SetInt("Weapon", currentWeapon.value);

            Debug.Log("Current level: " + currentLevelIndex.value + "\nCurrently equipped weapon: " + currentWeapon.value + "\nNumber of weapons Unlocked: " + unlockedWeapons.Count);
        }
    }

    public static void Save()
    {
        Debug.Log("Saving...");
        Debug.Log("Current level: " + currentLevelIndex + "\nCurrently equipped weapon: " + currentWeapon + "\nNumber of weapons Unlocked: " + unlockedWeapons.Count);
        List<string> jsonLines = new List<string>();
        
        jsonLines.Add(JsonUtility.ToJson(currentLevelIndex));
        jsonLines.Add(JsonUtility.ToJson(currentWeapon));
        jsonLines.Add(JsonUtility.ToJson(currentDifficulty));

        foreach(sInt w in unlockedWeapons)
        {
            jsonLines.Add(JsonUtility.ToJson(w));
        }

        File.WriteAllLines(saveFile, jsonLines);
    }

    public static void ClearSave()
    {
        File.Delete(saveFile);
    }

    #region setters
    public static void updateLevel(int level)
    {
        currentLevelIndex.value = level;
    }

    public static void addWeaponUnlock(sInt weapon)
    {
        unlockedWeapons.Add(weapon);
    }
    public static void addWeaponUnlock(int weapon)
    {
        unlockedWeapons.Add(new sInt(weapon));
    }

    public static void updateCurrentDifficulty(int diffuculty)
    {
        currentDifficulty.value = diffuculty;
    }

    public static void updateCurrentWeapon(int weapon)
    {
        currentWeapon.value = weapon;
    }
    #endregion

    #region getters
    public static bool getSaveExists()
    {
        return File.Exists(saveFile);
    }

    public static int getDifficulty()
    {
        return currentDifficulty.value;
    }

    public static int getCurrentWeapon()
    {
        return currentWeapon.value;
    }

    public static int getLevel()
    {
        return currentLevelIndex.value;
    }
    #endregion

}
