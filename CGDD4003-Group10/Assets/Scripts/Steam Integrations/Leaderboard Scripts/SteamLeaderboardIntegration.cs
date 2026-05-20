using Steamworks;
using Steamworks.Data;
using System;
using System.Threading.Tasks;
using UnityEngine;

public static class SteamLeaderboardIntegrations
{
  public static async Task<LeaderboardEntry[]> GetGlobalTop10(Leaderboard leaderboard)
  {
    try
    {
      LeaderboardEntry[] entries = await leaderboard.GetScoresAsync(10);
      return entries;
    }
    catch (Exception e)
    {
      Debug.LogError(e.Message);
    }
    return null;
  }

  /// <summary>
  ///     This function will only replace your last score if the new one is better.
  /// </summary>
  /// <param name="leaderboard"></param>
  /// <param name="value"></param>
  /// <param name="details"></param>
  public static async Task SubmitToLeaderboard(Leaderboard leaderboard, int value, int[] details = null)
  {
    var leaderboardUpdate = await leaderboard.SubmitScoreAsync(value, details ?? Array.Empty<int>());
    if (!leaderboardUpdate.HasValue)
    {
      Debug.LogError("leaderboardUpdate is null");
      return;
    }

    Debug.Log(leaderboardUpdate.Value);
  }

  public static async Task<Leaderboard?> GetLeaderBoards(string leaderboardName)
  {
    try
    {
      return await FindLeaderboardAsync(leaderboardName);
    }
    catch (Exception e)
    {
      Debug.LogError(e);
    }

    return null;
  }

  private static async Task<Leaderboard?> FindLeaderboardAsync(string leaderboardName)
  {
    try
    {
      return await SteamUserStats.FindLeaderboardAsync(leaderboardName);
    }
    catch (Exception e)
    {
      Debug.LogError(e);
    }

    return null;
  }
}
