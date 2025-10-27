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


        if(Steamworks.SteamClient.IsValid && Steamworks.SteamClient.IsLoggedOn)
        {
            connectedToSteam = true;
        }
        else
        {
            try
            {
                Steamworks.SteamClient.Init(appID, false);
                Debug.Log("connected to steamworks");
                connectedToSteam = true;
            }
            catch (System.Exception e)
            {
                connectedToSteam = false;
                Debug.Log("Failed to connect to steamworks");
                Debug.LogException(e);
            }
        }

        //Achievement Sync
        foreach (Achievement a in AchievementManager.getCompletedAchievements())
        {
            this.checkCompletedUnlockForSync(a.api_name);
        }
        foreach (Achievement a in AchievementManager.getPotentialAchievements())
        {
            if (this.checkPotentialUnlockForSync(a.api_name))
            {
                AchievementManager.stealthCollectAchievement(a);
            }
        }
        //ResetAchievements();
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
            try
            {
                Steamworks.SteamClient.RunCallbacks();
            }
            catch (System.Exception e)
            {
                Debug.Log($"Some Steam error occured - {e.Message}");
            }
        }
    }

    public void UnlockAchievement(string apiName)
    {
        Debug.Log($"Calling steamworks for {apiName} Achievement");
        if(connectedToSteam)
        {
            try
            {
                var achievement = new Steamworks.Data.Achievement(apiName);
                achievement.Trigger();
            }
            catch (System.Exception e)
            {
                Debug.Log($"Some Steam error occured - {e.Message}");
            }
        }
        else
        {
            Debug.Log("Unable to connect to steamworks");
        }
    }

    public void addDeath()
    {
        if (connectedToSteam)
        {
            try
            {
                Steamworks.SteamUserStats.RequestCurrentStats();
                if (Steamworks.SteamUserStats.GetStatInt("deaths") < 100)
                {
                    Steamworks.SteamUserStats.AddStat("deaths", 1);
                    Steamworks.SteamUserStats.StoreStats();
                    Steamworks.SteamUserStats.IndicateAchievementProgress("oof", Steamworks.SteamUserStats.GetStatInt("deaths"), 100);
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("Reached max deaths");
            }
        }
    }

    public void addFruit()
    {
        if (connectedToSteam)
        {
            try
            {
                Steamworks.SteamUserStats.RequestCurrentStats();
                if (Steamworks.SteamUserStats.GetStatInt("fruits") < 9)
                {
                    Steamworks.SteamUserStats.AddStat("fruits", 1);
                    Steamworks.SteamUserStats.StoreStats();
                    Steamworks.SteamUserStats.IndicateAchievementProgress("nom", Steamworks.SteamUserStats.GetStatInt("fruits"), 9);
                }
            }
            catch(System.Exception e)
            {
                Debug.Log("Reached max fruits");
            }
        }
    }

    public void addEnding()
    {
        if (connectedToSteam)
        {
            try
            {
                Steamworks.SteamUserStats.RequestCurrentStats();
                if (Steamworks.SteamUserStats.GetStatInt("endings") < 3)
                {
                    Steamworks.SteamUserStats.AddStat("endings", 1);
                    Steamworks.SteamUserStats.StoreStats();
                    Steamworks.SteamUserStats.IndicateAchievementProgress("triple_threat", Steamworks.SteamUserStats.GetStatInt("endings"), 3);
                }
            }
            catch(System.Exception e)
            {
                Debug.Log("Reached max endings");
            }
        }
    }

    public void checkCompletion()
    {
        if (connectedToSteam)
        {
            try
            {
                int numberUnlocked = 0;
                int totalAchievements = 0;
                Steamworks.SteamUserStats.RequestCurrentStats();
                foreach (Steamworks.Data.Achievement a in Steamworks.SteamUserStats.Achievements)
                {
                    totalAchievements++;
                    if (a.State)
                    {
                        numberUnlocked++;
                    }
                }
                Steamworks.SteamUserStats.SetStat("achievements", numberUnlocked);
                Steamworks.SteamUserStats.StoreStats();
                Steamworks.SteamUserStats.IndicateAchievementProgress("completed", Steamworks.SteamUserStats.GetStatInt("achievements"), totalAchievements);
            }
            catch(System.Exception e)
            {
                Debug.Log($"Some Steam error occured - {e.Message}");
            }
        }
    }

    public void checkCompletedUnlockForSync(string apiName)
    {
        if (connectedToSteam)
        {
            try
            {
                var achievement = new Steamworks.Data.Achievement(apiName);
                Debug.Log("Syncing Achievement: " + achievement.Name);
                if (!achievement.State)
                {
                    achievement.Trigger();
                }
            }
            catch(System.Exception e)
            {
                Debug.Log($"Some Steam error occured - {e.Message}");
            }
        }
    }
    public bool checkPotentialUnlockForSync(string apiName)
    {
        if (connectedToSteam)
        {
            try
            {
                var achievement = new Steamworks.Data.Achievement(apiName);
                Debug.Log("Syncing Achievement: " + achievement.Name);
                return achievement.State;
            }
            catch (System.Exception e)
            {
                Debug.Log($"Some Steam error occured - {e.Message}");
            }
        }
        return false;
    }

    // Call this before exiting the game
    public void DisconnectFromSteam()
    {
        if (connectedToSteam)
        {
            try
            {
                Debug.Log("Disconnecting from steamworks");
                Steamworks.SteamClient.Shutdown();
            }
            catch(System.Exception e)
            {
                Debug.Log($"Some Steam error occured - {e.Message}");
            }
        }
    }

    //RESETS ACHIEVEMENT PROGRESS DEV USE ONLY
    //THIS WILL CLEAR ALL STATS AND ACHIEVEMENTS
    public static void ResetAchievements()
    {
        if (connectedToSteam)
        {
            Steamworks.SteamUserStats.ResetAll(true);
            Debug.Log("Reset Achievements");
        }
    }
}
