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

    [SerializeField] private TextMeshProUGUI levelIndexText, levelNameText;
    [SerializeField] private RawImage[] levelStars;
    [SerializeField] private RawImage levelLock;

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

        /*
        for (int i = 0; i < levelStars.Length; i++)
        {
            if (i < levelData.levelStars)
            {
                levelStars[i].gameObject.SetActive(true);
            }
            else
            {
                levelStars[i].gameObject.SetActive(false);
            }
        }

        if (levelData.levelIndex > PlayerPrefs.GetInt("levelReached", 1))
        {
            levelLock.gameObject.SetActive(true);
        }
        else
        {
            levelLock.gameObject.SetActive(false);
        }
        */
    }
}