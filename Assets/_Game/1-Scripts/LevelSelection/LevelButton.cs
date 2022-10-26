using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public LevelSO levelData;
    public int levelIndex;
    public CampaignSO campaignData;

    [SerializeField] private TextMeshProUGUI levelIndexText, levelNameText;
    [SerializeField] private Image[] levelStars;
    [SerializeField] private RawImage levelLock;

    [SerializeField] private Sprite starEmpty, starFilled;

    [SerializeField] private SmartData.SmartEvent.EventDispatcher onLevelTransition;
    [SerializeField] private SmartData.SmartInt.IntWriter instanceLevelTransitionState;

    private void OnValidate()
    {
        UpdateData();
    }

    private void Start()
    {
        var button = GetComponent<Button>();
        //detect if the button is selected
    }

    public void Test()
    {
        print("test");
    }

    public void UpdateData()
    {
        levelIndexText.text = "#" + levelIndex.ToString();
        levelNameText.text = levelData.sceneName;

        levelLock.gameObject.SetActive(false);

        if (levelIndex > 0)
            if (campaignData.levels[levelIndex - 1].stars == 0)
                levelLock.gameObject.SetActive(true);


        for (var i = 0; i < levelStars.Length; i++)
            if (i < levelData.stars)
                levelStars[i].sprite = starEmpty;
            else
                levelStars[i].sprite = starFilled;
    }

    public void LaunchLevel()
    {
        if (levelLock.gameObject.activeSelf == false)
        {
            print("Button: loading level " + levelData.sceneName);
            instanceLevelTransitionState.value = (int)LevelLoader.LevelTransitionState.SpecificLevel;
            LevelLoader.SceneToLoad = levelData.scene;
            onLevelTransition.Dispatch();
        }
    }
}