using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreEntry
{
  public int playerRank;
  public int playerScore;
  public Friend User;
  public string name;

  public HighScoreEntry(int rank, Friend user, int score)
  {
    playerRank = rank;
    User = user;
    name = User.Name;
    playerScore = score;
  }

  public override string ToString()
  {
    var formatted = string.Format("{0,-4}{1,4}", $"{this.playerRank}.", this.name) + string.Format("{0,10}", this.playerScore);
    return formatted;
  }
  public string ToFileFormat()
  {
    return this.playerRank + " " + this.name + " " + this.playerScore;
  }
}
