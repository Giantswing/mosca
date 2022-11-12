using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class WinScreenScript : MonoBehaviour
{
    private GameObject _firstSelected;
    [SerializeField] private UIAnimator _uiAnimator;

    [SerializeField] private TextMeshProUGUI levelNameText, scoreText, timeText;
    [SerializeField] private RawImage winScreenImage, winScreenBG;
    [SerializeField] private Image[] starsImage;
    [SerializeField] private Sprite starFilled, starEmpty, starNew;
    [SerializeField] private RectTransform scoreParent, timeParent, starsParent;
    [SerializeField] private GameObject nextLevelButton;
    [SerializeField] private GameObject levelSelectionButton;

    [Space(10)] [Header("Sound")] [SerializeField]
    private SimpleAudioEvent startWinScreenAudioEvent;

    [SerializeField] private SimpleAudioEvent winStarAudioEvent;

    private WaitForSecondsRealtime wait_sm = new(.2f);
    private WaitForSecondsRealtime wait_md = new(.35f);
    private WaitForSecondsRealtime wait_lg = new(1f);
    private WaitForSecondsRealtime wait_xl = new(1.2f);

    [Space(25)] [SerializeField] private float singleDuration = .2f;
    [SerializeField] private float singleHideDuration = .2f;
    [SerializeField] private float singleDelay = .2f;
    [SerializeField] private float singleHideDelay = .2f;
    [SerializeField] private AnimationCurve singleSpawnCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField] private RectTransform[] _children;

    [Header("Level Loader Settings")] [Space(10)] [SerializeField]
    private SmartData.SmartInt.IntWriter instanceLevelTransitionState;

    [SerializeField] private SmartData.SmartEvent.EventDispatcher onWinScreen;
    [SerializeField] private SmartData.SmartBool.BoolWriter finishTransition;
    [SerializeField] private CampaignSO campaignData;
    private bool _alreadyInit = false;

    [SerializeField] private SmartData.SmartFloat.FloatReader levelTime;
    [SerializeField] private SmartData.SmartFloat.FloatReader levelTimeMax;


    private void Start()
    {
        if (CurrentLevelHolder.GetCurrentLevel().isThisLastOne)
        {
            nextLevelButton.SetActive(false);
            _firstSelected = levelSelectionButton;
        }
        else
        {
            _firstSelected = nextLevelButton;
        }

        _children = GetComponentsInChildren<RectTransform>();

        for (var i = 0; i < _children.Length; i++) _children[i].gameObject.SetActive(false);

        gameObject.SetActive(true);
    }

    public void InitWinScreenAnimation()
    {
        if (instanceLevelTransitionState == (int)LevelLoader.LevelTransitionState.DontLoadYet && !_alreadyInit)
        {
            _alreadyInit = true;
            StartCoroutine(StartWinScreenAnimationRoutine());
        }
    }

    private IEnumerator StartWinScreenAnimationRoutine()
    {
        yield return new WaitForSeconds(.5f);
        GlobalAudioManager.PlaySound(startWinScreenAudioEvent);

        var currentLevel = CurrentLevelHolder.GetCurrentLevel();
        levelNameText.SetText(currentLevel.sceneName);
        scoreText.SetText(LevelManager.GetScore().ToString() + "/" + currentLevel.totalScore);
        timeText.SetText(levelTime.value.ToString("F1") + "s/" + levelTimeMax.value + "s");

        var currentStarsInLevel = currentLevel.stars;
        var starsWonInLevel = 0;

        if (LevelManager.GetScore() >= currentLevel.scoreToWin)
            starsWonInLevel++;

        if (LevelManager.GetScore() == currentLevel.totalScore)
            starsWonInLevel++;

        if (levelTime.value < currentLevel.timeToWin)
            starsWonInLevel++;


        for (var i = 1; i <= 3; i++)
        {
            starsImage[i - 1].sprite = starEmpty;

            if (starsWonInLevel >= i) starsImage[i - 1].sprite = starNew;

            if (currentStarsInLevel >= i) starsImage[i - 1].sprite = starFilled;
        }

        currentLevel.stars = starsWonInLevel;

        SaveLoadSystem.SaveGame();
        StartCoroutine(StarSoundsRoutine(starsWonInLevel));

        /*
        _uiAnimator.StartAnimation(_children, singleDuration, singleDelay, singleSpawnCurve,
            () => { EventSystemScript.ChangeFirstSelected(firstSelected); });
            */

        _uiAnimator.StartAnimation(_children, singleDuration, singleDelay, singleSpawnCurve,
            () => { EventSystemScript.ChangeFirstSelected(_firstSelected); });
    }

    private IEnumerator StarSoundsRoutine(int stars)
    {
        yield return wait_xl;

        for (var i = 0; i < stars; i++)
        {
            winStarAudioEvent.pitch.minValue = 1f + 0.1f * i;
            winStarAudioEvent.pitch.maxValue = winStarAudioEvent.pitch.minValue;
            GlobalAudioManager.PlaySound(winStarAudioEvent);
            yield return wait_sm;
        }
    }

    private void HideWinScreenAnimation(string menuAction)
    {
        switch (menuAction)
        {
            case "restart_level":
                DOTween.KillAll();
                print(menuAction);
                instanceLevelTransitionState.value = (int)LevelLoader.LevelTransitionState.Restart;
                break;
            case "go_to_menu":
                DOTween.KillAll();
                print(menuAction);
                instanceLevelTransitionState.value = (int)LevelLoader.LevelTransitionState.SpecificLevel;
                LevelLoader.SceneToLoad = campaignData.levelSelectionScene;
                break;
            case "next_level":
                DOTween.KillAll();
                print(menuAction);
                instanceLevelTransitionState.value = (int)LevelLoader.LevelTransitionState.NextLevel;
                break;
        }

        onWinScreen.Dispatch();

        _uiAnimator.ReverseAnimation(_children, singleHideDuration, singleHideDelay, Ease.OutQuad,
            () => { finishTransition.value = true; });
    }


    public void ButtonRestartLevel()
    {
        HideWinScreenAnimation("restart_level");
    }

    public void ButtonMenu()
    {
        HideWinScreenAnimation("go_to_menu");
    }

    public void ButtonNextLevel()
    {
        HideWinScreenAnimation("next_level");
    }


    public void MoveActiveButton(InputAction.CallbackContext context)
    {
        print(context.ReadValue<Vector2>());
    }
}