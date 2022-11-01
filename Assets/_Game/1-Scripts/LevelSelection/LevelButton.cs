using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public LevelSO levelData;
    public int levelIndex;
    public bool isBLevel = false;
    public CampaignSO campaignData;

    [SerializeField] private TextMeshProUGUI levelIndexText, levelNameText;
    [SerializeField] private Image[] levelStars;
    [SerializeField] private RawImage levelLock;

    [SerializeField] private Sprite starEmpty, starFilled;

    [SerializeField] private SmartData.SmartEvent.EventDispatcher onLevelTransition;
    [SerializeField] private SmartData.SmartInt.IntWriter instanceLevelTransitionState;

    private Button button;
    [SerializeField] private LineRenderer lineRenderer;

    [HideInInspector] public LevelSelectionManager levelSelectionManager;

    public RectTransform rectTransform;
    private RectTransform _otherRecTransform;

    // private void OnValidate()
    // {
    //     UpdateData();
    // }

    private void Start()
    {
        button = GetComponent<Button>();
        StartCoroutine(Test());
        //_otherRecTransform = levelSelectionManager.buttons[levelIndex - 1].position;
    }

    private IEnumerator Test()
    {
        yield return new WaitForSeconds(0.1f);
        UpdateData();
    }

    public void UpdateData()
    {
        if (!isBLevel)
            levelIndexText.SetText(levelIndex.ToString());
        else
            levelIndexText.SetText(levelIndex.ToString() + "B");

        levelNameText.text = levelData.sceneName;

        levelLock.gameObject.SetActive(false);

        if (levelIndex > 0)
            if (campaignData.levels[levelIndex - 1].stars == 0)
                levelLock.gameObject.SetActive(true);

        if (isBLevel)
            if (campaignData.levels[levelIndex].stars == 0)
                levelLock.gameObject.SetActive(true);


        for (var i = 0; i < levelStars.Length; i++)
            if (i < levelData.stars)
                levelStars[i].sprite = starEmpty;
            else
                levelStars[i].sprite = starFilled;


        if (levelIndex > 0)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, levelSelectionManager.buttons[levelIndex].position);
            lineRenderer.SetPosition(1,
                levelSelectionManager.buttons[levelIndex - 1].position);
        }

        if (isBLevel)
            transform.position = new Vector3(transform.position.x,
                levelSelectionManager.buttons[levelIndex].position.y - 70, transform.position.z);
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

    private void Update()
    {
        if (levelIndex > 0)
        {
            var offset = 10f;
            lineRenderer.positionCount = 2;
            if (!isBLevel)
            {
                lineRenderer.SetPosition(0,
                    new Vector3(levelSelectionManager.buttons[levelIndex].position.x,
                        levelSelectionManager.buttons[levelIndex].position.y,
                        levelSelectionManager.buttons[levelIndex].position.z + offset));
                lineRenderer.SetPosition(1, new Vector3(
                    levelSelectionManager.buttons[levelIndex - 1].position.x,
                    levelSelectionManager.buttons[levelIndex - 1].position.y,
                    levelSelectionManager.buttons[levelIndex - 1].position.z + offset));
            }
            else
            {
                lineRenderer.SetPosition(0,
                    new Vector3(button.transform.position.x, button.transform.position.y,
                        button.transform.position.z + offset));
                lineRenderer.SetPosition(1, new Vector3(
                    levelSelectionManager.buttons[levelIndex].position.x,
                    levelSelectionManager.buttons[levelIndex].position.y,
                    levelSelectionManager.buttons[levelIndex].position.z + offset));
            }
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        transform.DOScale(1.1f, 0.2f);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        transform.DOScale(1f, 0.2f);
    }
}