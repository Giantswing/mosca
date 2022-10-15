using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.SceneManagement;
using Utilities;

[CreateAssetMenu(fileName = "Campaign", menuName = "Mosca/Campaign", order = 1)]
public class CampaignSO : ScriptableObject
{
    public SceneField levelSelectionScene;
    public List<LevelSO> levels;

    public int GetLevelIndex(LevelSO level)
    {
        return levels.IndexOf(level);
    }

    public List<LevelSO> GetLevels()
    {
        return levels;
    }

    public void ResetAllStars()
    {
        for (var i = 0; i < levels.Count; i++) levels[i].stars = 0;
    }

    public LevelSO GetCurrentLevel(string sceneName)
    {
        var j = 0;
        for (var i = 0; i < levels.Count; i++)
            if (levels[i].scene.SceneName == sceneName)
                j = i;

        return levels[j];
    }
}