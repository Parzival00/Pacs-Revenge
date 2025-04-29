using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "PlayerStats")]
public class PlayerStats : ScriptableObject
{
    public int score { get; private set; }
    public int fruitsCollected { get; private set; }
    public int currentLevel { get; private set; }
    public int difficulty { get; private set; }

    public int totalGhostKilled { get; set; }
    public int totalPelletsCollected { get; private set; }
    public int totalShieldsRecieved { get; set; }
    public int totalLivesConsumed { get; set; }
    public int totalShotsFired { get; set; }
    public int totalStunsFired { get; set; }
    public float totalTimePlayed { get; private set; }
    public int timesOverheated { get; set; }
}
