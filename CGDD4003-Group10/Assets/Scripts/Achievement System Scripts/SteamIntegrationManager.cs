using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamIntegrationManager : MonoBehaviour
{
    public static SteamIntegrationManager instance;
    private readonly uint appID = 3572570;
    private bool connectedToSteam = false;

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

    public void UnlockAchievement(string name)
    {
        Debug.Log($"Calling steamworks for {name} Achievement");
        if(connectedToSteam)
        {
            var achievement = new Steamworks.Data.Achievement(name);
            achievement.Trigger();
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
}
