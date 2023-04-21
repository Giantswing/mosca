using System;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveLoadSystem : MonoBehaviour
{
    [SerializeField] private CampaignSO campaign;
    public static SaveLoadSystem Instance { get; private set; }

    private void OnEnable()
    {
        Instance = this;
    }

    public static void SaveGame()
    {
        PlayerPrefs.SetInt("TotalHearts", Instance.campaign.heartContainers);
        PlayerPrefsX.SetIntArray("HeartIds", Instance.campaign.heartContainerIDs.ToArray());


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
        Instance.campaign.heartContainers = PlayerPrefs.GetInt("TotalHearts", 0);

        Instance.campaign.heartContainerIDs.Clear();
        Instance.campaign.heartContainerIDs.AddRange(PlayerPrefsX.GetIntArray("HeartIds", 0, 0));

        for (var i = 0; i < Instance.campaign.levels.Count; i++)
        {
            bool hasBSide = Instance.campaign.levels[i].hasBSide;
            int data = PlayerPrefs.GetInt(Instance.campaign.levels[i].sceneName, 0);
            int bdata = hasBSide ? PlayerPrefs.GetInt(Instance.campaign.levels[i].bSideScene.sceneName, 0) : 0;

            //loading stars
            Instance.campaign.levels[i].stars = data;

            if (hasBSide)
                Instance.campaign.levels[i].bSideScene.stars = bdata;

            //loading deaths

            int deaths = PlayerPrefs.GetInt(Instance.campaign.levels[i].sceneName + "deaths", 0);

            int bdeaths = hasBSide
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
}