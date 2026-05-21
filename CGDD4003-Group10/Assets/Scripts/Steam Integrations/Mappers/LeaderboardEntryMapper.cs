using Steamworks.Data;

public class LeaderboardEntryMapper
{
  public HighScoreEntry MapLbEntryToHsEntry(LeaderboardEntry lbEntry)
  {
    var hsEntry = new HighScoreEntry(
      lbEntry.GlobalRank,
      lbEntry.User,
      lbEntry.Score
      );
    return hsEntry;
  }
}
