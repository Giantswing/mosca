using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveLoadSystem : MonoBehaviour
{
    [SerializeField] private CampaignSO campaign;
    public static SaveLoadSystem Instance { get; private set; }

    [SerializeField] private InputAction quitAction;

    private void Awake()
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
        print("bye bye");
        Application.Quit();
    }
}