using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks.Data;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
  private const string LeaderboardName = "High-Scores";
  private Leaderboard _Leaderboard;
  private async void Awake()
  {
    try
    {
      var lb = await SteamLeaderboardIntegrations.GetLeaderBoards(LeaderboardName);
      if (lb.HasValue)
      {
        _Leaderboard = lb.Value;
      }
      else
      {
        Debug.Log("Leaderboard returned null or empty value");
      }
    }
    catch (Exception  ex) 
    {
      Debug.LogError(ex.Message);
    }
  }
  
}
