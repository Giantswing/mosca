using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentLevelHolder : MonoBehaviour
{
    [SerializeField] private LevelSO currentLevel;
    public static CurrentLevelHolder Instance;


    private void Awake()
    {
        Instance = this;
        //get the current scene name

        //get the level data from the level list
        //currentLevel = LevelList.Instance.GetLevelDataFromSceneName(SceneManager.GetActiveScene().name);
    }

    public static LevelSO GetCurrentLevel()
    {
        if (Instance != null)
            return Instance.currentLevel;
        else
            return null;
    }
}