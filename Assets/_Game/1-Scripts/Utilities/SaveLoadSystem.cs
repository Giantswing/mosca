using System;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveLoadSystem : MonoBehaviour
{
    [SerializeField] private CampaignSO campaign;
    public static SaveLoadSystem Instance { get; private set; }

    [SerializeField] private InputAction quitAction;

    [SerializeField] private SmartData.SmartInt.IntWriter instanceLevelTransitionState;
    [SerializeField] private SmartData.SmartEvent.EventDispatcher onWinScreen;
    [SerializeField] private SmartData.SmartBool.BoolWriter finishTransition;

    private void OnEnable()
    {
        Instance = this;

        quitAction.performed += _ => QuitGame();
        quitAction.Enable();
    }

    private void OnDisable()
    {
        quitAction.Disable();
    }

    public static void SaveGame()
    {
        for (var i = 0; i < Instance.campaign.levels.Count; i++)
        {
            PlayerPrefs.SetInt(Instance.campaign.levels[i].sceneName, Instance.campaign.levels[i].stars);
            PlayerPrefs.SetInt(Instance.campaign.levels[i].sceneName + "deaths",
                Instance.campaign.levels[i].deathCounter);

            if (Instance.campaign.levels[i].hasBSide)
            {
                PlayerPrefs.SetInt(Instance.campaign.levels[i].bSideScene.sceneName,
                    Instance.campaign.levels[i].bSideScene.stars);
                PlayerPrefs.SetInt(Instance.campaign.levels[i].bSideScene.sceneName + "deaths",
                    Instance.campaign.levels[i].bSideScene.deathCounter);
            }
        }
    }

    public static void LoadGame()
    {
        for (var i = 0; i < Instance.campaign.levels.Count; i++)
        {
            var hasBSide = Instance.campaign.levels[i].hasBSide;
            var data = PlayerPrefs.GetInt(Instance.campaign.levels[i].sceneName, 0);
            var bdata = hasBSide ? PlayerPrefs.GetInt(Instance.campaign.levels[i].bSideScene.sceneName, 0) : 0;

            //loading stars
            Instance.campaign.levels[i].stars = data;

            if (hasBSide)
                Instance.campaign.levels[i].bSideScene.stars = bdata;

            //loading deaths

            var deaths = PlayerPrefs.GetInt(Instance.campaign.levels[i].sceneName + "deaths", 0);

            var bdeaths = hasBSide
                ? PlayerPrefs.GetInt(Instance.campaign.levels[i].bSideScene.sceneName + "deaths", 0)
                : 0;

            Instance.campaign.levels[i].deathCounter = deaths;

            if (hasBSide)
                Instance.campaign.levels[i].bSideScene.deathCounter = bdeaths;
        }
    }


    public static void DeleteSavedGame()
    {
        PlayerPrefs.DeleteAll();
        LoadGame();
        print("Deleted");
    }

    private void QuitGame()
    {
        //check current scene, if it is the level selection then go to main menu
        print(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "_levelSelection")
        {
        }

        DOTween.KillAll();
        instanceLevelTransitionState.value = (int)LevelLoader.LevelTransitionState.SpecificLevel;

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "_levelSelection")
            LevelLoader.SceneToLoad = campaign.mainMenuScene;
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "main-menu")
            Application.Quit();
        else
            LevelLoader.SceneToLoad = campaign.levelSelectionScene;

        onWinScreen.Dispatch();

        finishTransition.value = true;
    }
}