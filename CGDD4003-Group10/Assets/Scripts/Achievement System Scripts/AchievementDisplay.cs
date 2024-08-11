using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementDisplay : MonoBehaviour
{
    [SerializeField] AchievementDisplayEntry[] achievementEntries;

    public void DisplayAchievements()
    {
        achievementEntries = GetComponentsInChildren<AchievementDisplayEntry>();

        List<Achievement> completed = AchievementManager.getCompletedAchievements();
        for (int i = 0; i < completed.Count; i++)
        {
            achievementEntries[i].Display(completed[i]);
        }
        List<Achievement> potential = AchievementManager.getPotentialAchievements();
        for (int i = 0; i < potential.Count; i++)
        {
            achievementEntries[completed.Count + i].Display(potential[i]);
        }
    }
}
