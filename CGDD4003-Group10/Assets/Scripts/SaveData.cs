using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public static class SaveData
{
    static string saveFile;
    static string weaponsFile;

    static List<sInt> unlockedWeapons = new List<sInt>();
    static sInt currentLevelIndex;
    static sInt currentWeapon;
    static sInt currentDifficulty;
    static sInt currentKillCount;
    static sInt currentScore;
    static sInt currentShotsFired;
    static sInt currentStuns;
    static sInt currentShieldsUsed;
    static sInt currentDeathCount;
    static sInt currentPelletsCollected;
    static sInt currentRunTime;

    static SaveData()
    {
        saveFile = Application.persistentDataPath + "/playerData.json";
        weaponsFile = Application.persistentDataPath + "/weapons.json";
        unlockedWeapons = new List<sInt>();

        currentLevelIndex = new sInt(0);
        currentWeapon = new sInt(0);
        currentDifficulty = new sInt(0);
        currentKillCount = new sInt(0);
        currentScore = new sInt(0);
        currentShotsFired = new sInt(0);
        currentStuns = new sInt(0);
        currentShieldsUsed = new sInt(0);
        currentDeathCount = new sInt(0);
        currentPelletsCollected = new sInt(0);
        currentRunTime = new sInt(0);



        if (File.Exists(weaponsFile))
        {
            string[] jsonLines = File.ReadAllLines(weaponsFile);

            for (int i = 0; i < jsonLines.Length; i++)
            {
                unlockedWeapons.Add(JsonUtility.FromJson<sInt>(jsonLines[i]));
            }
        }
        else
        {
            unlockedWeapons.Add(new sInt(0));
            List<string> jsonLines = new List<string>();
            jsonLines.Add(JsonUtility.ToJson(unlockedWeapons[0]));
            File.WriteAllLines(weaponsFile, jsonLines);
        }

        Debug.Log(saveFile);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Load()
    {
        Debug.Log(weaponsFile);
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
            currentKillCount = JsonUtility.FromJson<sInt>(jsonLines[lineIndex++]);
            currentScore = JsonUtility.FromJson<sInt>(jsonLines[lineIndex++]);
            currentShotsFired = JsonUtility.FromJson<sInt>(jsonLines[lineIndex++]);
            currentStuns = JsonUtility.FromJson<sInt>(jsonLines[lineIndex++]);
            currentShieldsUsed = JsonUtility.FromJson<sInt>(jsonLines[lineIndex++]);
            currentDeathCount = JsonUtility.FromJson<sInt>(jsonLines[lineIndex++]);
            currentPelletsCollected = JsonUtility.FromJson<sInt>(jsonLines[lineIndex++]);
            currentRunTime = JsonUtility.FromJson<sInt>(jsonLines[lineIndex++]);

            //PlayerPrefs.SetInt("Weapon", currentWeapon.value);

            getPlayerMetrics();

            Debug.Log("Current level: " + currentLevelIndex.value + "\nCurrently equipped weapon: " + currentWeapon.value + "\nNumber of weapons Unlocked: " + unlockedWeapons.Count);
        }
    }

    public static void Save()
    {
        Debug.Log("Saving...");
        Debug.Log("Current level: " + currentLevelIndex + "\nCurrently equipped weapon: " + currentWeapon + "\nNumber of weapons Unlocked: " + unlockedWeapons.Count);

        ClearSave();

        List<string> jsonLines = new List<string>();
        
        jsonLines.Add(JsonUtility.ToJson(currentLevelIndex));
        jsonLines.Add(JsonUtility.ToJson(currentWeapon));
        jsonLines.Add(JsonUtility.ToJson(currentDifficulty));
        jsonLines.Add(JsonUtility.ToJson(currentKillCount));
        jsonLines.Add(JsonUtility.ToJson(currentScore));
        jsonLines.Add(JsonUtility.ToJson(currentShotsFired));
        jsonLines.Add(JsonUtility.ToJson(currentStuns));
        jsonLines.Add(JsonUtility.ToJson(currentShieldsUsed));
        jsonLines.Add(JsonUtility.ToJson(currentDeathCount));
        jsonLines.Add(JsonUtility.ToJson(currentPelletsCollected));
        jsonLines.Add(JsonUtility.ToJson(currentRunTime));

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
        bool unlockedAlready = false;
        foreach(sInt w in unlockedWeapons)
        {
            if (w.value == weapon.value)
            {
                unlockedAlready = true;
            }
        }

        if (!unlockedAlready)
        {
            unlockedWeapons.Add(weapon);

            List<string> jsonLines = new List<string>();
            jsonLines.Add(JsonUtility.ToJson(weapon));
            File.WriteAllLines(weaponsFile, jsonLines);
        }
    }
    public static void addWeaponUnlock(int weapon)
    {
        bool unlockedAlready = false;
        foreach (sInt w in unlockedWeapons)
        {
            if (w.value == weapon)
            {
                unlockedAlready = true;
            }
        }

        if (!unlockedAlready)
        {
            sInt newWeapon = new sInt(weapon);
            unlockedWeapons.Add(newWeapon);

            List<string> jsonLines = new List<string>();
            jsonLines.Add(JsonUtility.ToJson(newWeapon));
            File.WriteAllLines(weaponsFile, jsonLines);
        }
    }

    public static void updateCurrentDifficulty(int diffuculty)
    {
        currentDifficulty.value = diffuculty;
    }

    public static void updateCurrentWeapon(int weapon)
    {
        currentWeapon.value = weapon;
    }

    public static void updateStatuses(int score, int kills, int shots, int stuns, int shields, int deaths, int pellets, int runtime)
    {
        currentScore = new sInt(score);
        currentKillCount = new sInt(kills);
        currentShotsFired = new sInt(shots);
        currentShieldsUsed = new sInt(shields);
        currentDeathCount = new sInt(deaths);
        currentDeathCount = new sInt(pellets);
        currentRunTime = new sInt(runtime);
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

    public static List<sInt> getWeaponsUnlocked()
    {
        return unlockedWeapons;
    }

    public static void getPlayerMetrics()
    {
        Debug.Log("Getting Player metrics");
        Score.UpdatePlayerStats(currentScore.value, currentKillCount.value, currentShotsFired.value, currentStuns.value, currentShieldsUsed.value, currentDeathCount.value, currentPelletsCollected.value, currentRunTime.value);
    }

    #endregion

}
