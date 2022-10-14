using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

[CreateAssetMenu(fileName = "Campaign", menuName = "Mosca/Campaign", order = 1)]
public class CampaignSO : ScriptableObject
{
    public SceneField levelSelectionScene;
    public List<LevelSO> level;

    public int GetLevelIndex(LevelSO level)
    {
        return this.level.IndexOf(level);
    }

    public List<LevelSO> GetLevels()
    {
        return level;
    }

    public void ResetAllStars()
    {
        for (var i = 0; i < level.Count; i++) level[i].stars = 0;
    }
}