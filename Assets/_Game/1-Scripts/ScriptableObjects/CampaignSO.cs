using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.SceneManagement;


[CreateAssetMenu(fileName = "Campaign", menuName = "Mosca/Campaign", order = 1)]
public class CampaignSO : ScriptableObject
{
    public SceneField levelSelectionScene;
    public SceneField mainMenuScene;
    public LevelSO defaultScene;
    public List<LevelSO> levels;

    [Space(25)] public int heartContainers = 0;
    public List<int> heartContainerIDs;

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
        for (var i = 0; i < levels.Count; i++)
        {
            levels[i].stars = 0;
            levels[i].deathCounter = 0;
        }

        SaveLoadSystem.SaveGame();
    }

    /*
    public LevelSO GetCurrentLevel(string sceneName)
    {
        var j = 0;

        for (var i = 0; i < levels.Count; i++)
        {
            var levelSceneName = levels[i].;

            levelSceneName = levels[i].scene.
                //select only the last part of the scene name
                levelSceneName = levelSceneName.Substring(levelSceneName.LastIndexOf("/") + 1);

            if (levelSceneName == sceneName)
                j = i;
        }

        return levels[j];
    }
    */

    public void UpdateLevelInfo()
    {
        for (var i = 0; i < levels.Count; i++)
        {
            levels[i].isThisLastOne = false;
            if (i == levels.Count - 1)
                levels[i].isThisLastOne = true;

            levels[i].index = i;
        }
    }
}