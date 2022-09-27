using System.Collections.Generic;
using UnityEngine;

public class GameDataStore : MonoBehaviour
{

    public List<LevelSaveData> completedLevels = new List<LevelSaveData>();


    public void PreSave()
    {
        Debug.Log("[GAME] Saving Game");
    }
    
    public void PostLoad()
    {
        Debug.Log("[GAME] Loaded Game");
    }
    
    public void CompleteLevel(string levelId, int starsEarned)
    {
        foreach (LevelSaveData level in completedLevels)
        {
            if (level.id == levelId)
            {
                level.numberOfStars = Mathf.Max(level.numberOfStars, starsEarned);
                return;
            }
        }
        completedLevels.Add(new LevelSaveData(levelId, starsEarned));
    }
    
    public bool IsLevelCompleted(string levelId)
    {
        foreach (LevelSaveData level in completedLevels)
        {
            if (level.id == levelId)
            {
                return true;
            }
        }
        return false;
    }
    
    public int GetNumberOfStarForLevel(string levelId)
    {
        foreach (LevelSaveData level in completedLevels)
        {
            if (level.id == levelId)
            {
                return level.numberOfStars;
            }
        }
        return 0;
    }
}
