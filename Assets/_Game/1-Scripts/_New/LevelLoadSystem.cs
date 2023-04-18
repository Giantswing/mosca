using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoadSystem : MonoBehaviour
{
    [SerializeField] private CampaignSO campaignData;
    private AsyncOperation _asyncLoad;
    private SceneField _sceneToLoad;

    public static LevelLoadSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public enum LevelToLoad
    {
        Restart,
        NextLevel,
        SpecificLevel,
        GoToMenu,
        LevelSelection,
        LoadNextInBuild,
        DontLoadYet
    }

    public static void LoadLevel(LevelToLoad transitionType)
    {
        Instance.LoadLevelInterface(transitionType);
    }

    public static void LoadSpecificLevel(SceneField sceneToLoad)
    {
        Instance._sceneToLoad = sceneToLoad;
        LoadLevel(LevelToLoad.SpecificLevel);
    }


    public void LoadLevelInterface(LevelToLoad transitionType)
    {
        switch (transitionType)
        {
            case LevelToLoad.Restart:
                _asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
                break;
            case LevelToLoad.NextLevel:
                int nextLevelIndex = LevelManager.GetCurrentLevel().index + 1;
                _sceneToLoad = campaignData.levels[nextLevelIndex].scene;
                _asyncLoad = SceneManager.LoadSceneAsync(_sceneToLoad.BuildIndex);
                break;
            case LevelToLoad.SpecificLevel:
                _asyncLoad = SceneManager.LoadSceneAsync(_sceneToLoad.BuildIndex);
                break;
            case LevelToLoad.GoToMenu:
                _sceneToLoad = campaignData.mainMenuScene;
                _asyncLoad = SceneManager.LoadSceneAsync(_sceneToLoad.BuildIndex);
                break;
            case LevelToLoad.LevelSelection:
                _sceneToLoad = campaignData.levelSelectionScene;
                _asyncLoad = SceneManager.LoadSceneAsync(_sceneToLoad.BuildIndex);
                break;
            case LevelToLoad.LoadNextInBuild:
                _asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
                break;
            case LevelToLoad.DontLoadYet:
                break;
        }

        _asyncLoad.allowSceneActivation = false;

        LevelTransitionScript.StartTransition(() => { _asyncLoad.allowSceneActivation = true; });
    }
}