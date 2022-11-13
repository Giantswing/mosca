using System;
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
        for (var j = 0; j < Instance.campaign.levels.Count; j++)
        {
            PlayerPrefs.SetInt(Instance.campaign.levels[j].sceneName, Instance.campaign.levels[j].stars);
            PlayerPrefs.SetInt(Instance.campaign.levels[j].sceneName + "deaths",
                Instance.campaign.levels[j].deathCounter);
        }
    }

    public static void LoadGame()
    {
        for (var i = 0; i < Instance.campaign.levels.Count; i++)
        {
            var data = PlayerPrefs.GetInt(Instance.campaign.levels[i].sceneName);
            if (data != 0)
                Instance.campaign.levels[i].stars = data;
            else Instance.campaign.levels[i].stars = 0;

            var deathCounterData = PlayerPrefs.GetInt(Instance.campaign.levels[i].sceneName + "deaths");
            if (data != 0)
            {
                print("found info");
                Instance.campaign.levels[i].deathCounter = deathCounterData;
            }
            else
            {
                Instance.campaign.levels[i].deathCounter = 0;
            }
        }
    }


    public static void DeleteSavedGame()
    {
        PlayerPrefs.DeleteAll();
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