using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamIntegrationManager : MonoBehaviour
{
    public static SteamIntegrationManager instance;
    private readonly uint appID = 3572570;
    private static bool connectedToSteam = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        try
        {
            Steamworks.SteamClient.Init(appID,false);
            connectedToSteam = true;
        }
        catch (System.Exception e)
        {
            connectedToSteam = false;
            Debug.Log("Failed to connect to steamworks");
            Debug.LogException(e);
        }

    }

    void OnApplicationQuit()
    {
        DisconnectFromSteam();
    }

    // Update is called once per frame
    void Update()
    {
        if (connectedToSteam)
        {
            Steamworks.SteamClient.RunCallbacks();
        }
    }

    public void UnlockAchievement(string apiName)
    {
        Debug.Log($"Calling steamworks for {apiName} Achievement");
        if(connectedToSteam)
        {
            var achievement = new Steamworks.Data.Achievement(apiName);
            achievement.Trigger();
        }
    }

    public void addDeath()
    {
        if (connectedToSteam)
        {
            Steamworks.SteamUserStats.RequestCurrentStats();
            Steamworks.SteamUserStats.AddStat("deaths", 1);
            Steamworks.SteamUserStats.StoreStats();
            Steamworks.SteamUserStats.IndicateAchievementProgress("oof", Steamworks.SteamUserStats.GetStatInt("deaths"), 100);
        }
    }

    public void addFruit()
    {
        if (connectedToSteam)
        {
            Steamworks.SteamUserStats.RequestCurrentStats();
            Steamworks.SteamUserStats.AddStat("fruits", 1);
            Steamworks.SteamUserStats.StoreStats();
            Steamworks.SteamUserStats.IndicateAchievementProgress("nom", Steamworks.SteamUserStats.GetStatInt("fruits"), 9);
        }
    }

    public void addEnding()
    {
        if (connectedToSteam)
        {
            Steamworks.SteamUserStats.RequestCurrentStats();
            Steamworks.SteamUserStats.AddStat("endings", 1);
            Steamworks.SteamUserStats.StoreStats();
            Steamworks.SteamUserStats.IndicateAchievementProgress("triple_threat", Steamworks.SteamUserStats.GetStatInt("endings"), 3);
        }
    }

    public void checkCompletion()
    {
        if (connectedToSteam)
        {
            int numberUnlocked = 0;
            int totalAchievements = 0;
            Steamworks.SteamUserStats.RequestCurrentStats();
            foreach (Steamworks.Data.Achievement a in Steamworks.SteamUserStats.Achievements)
            {
                totalAchievements++;
                if(a.State)
                {
                    numberUnlocked++;
                }
            }
            Steamworks.SteamUserStats.SetStat("achievements", numberUnlocked);
            Steamworks.SteamUserStats.StoreStats();
            Steamworks.SteamUserStats.IndicateAchievementProgress("completed", Steamworks.SteamUserStats.GetStatInt("achievements"), totalAchievements);
        }
    }

    // Call this before exiting the game
    public void DisconnectFromSteam()
    {
        if (connectedToSteam)
        {
            Steamworks.SteamClient.Shutdown();
        }
    }

    //RESETS ACHIEVEMENT PROGRESS DEV USE ONLY
    //THIS WILL CLEAR ALL STATS AND ACHIEVEMENTS
    public static void ResetAchievements()
    {
        if (connectedToSteam)
        {
            Steamworks.SteamUserStats.RequestCurrentStats();
            Steamworks.SteamUserStats.ResetAll(true);
        }
    }
}
