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

    private WaitForSecondsRealtime wait_sm = new(.2f);
    private WaitForSecondsRealtime wait_md = new(.35f);
    private WaitForSecondsRealtime wait_lg = new(1f);
    private WaitForSecondsRealtime wait_xl = new(1.5f);

    [SerializeField] private float singleDuration = .2f;
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

    private void Start()
    {
        if (LevelLoader.IsThisLastLevel())
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

        levelNameText.SetText(LevelManager.LevelData().sceneName);
        scoreText.SetText(LevelManager.GetScore().ToString() + "/" + LevelManager.LevelData().totalScore);
        timeText.SetText(LevelManager.LevelTime.ToString("F1") + "s/" + LevelManager.LevelData().timeToWin + "s");

        var currentStarsInLevel = LevelManager.LevelData().stars;
        var starsWonInLevel = 0;

        if (LevelManager.GetScore() >= LevelManager.LevelData().scoreToWin)
            starsWonInLevel++;

        if (LevelManager.GetScore() == LevelManager.LevelData().totalScore)
            starsWonInLevel++;

        if (LevelManager.LevelTime < LevelManager.LevelData().timeToWin)
            starsWonInLevel++;


        for (var i = 1; i <= 3; i++)
        {
            starsImage[i - 1].sprite = starEmpty;

            if (starsWonInLevel >= i)
                starsImage[i - 1].sprite = starNew;

            if (currentStarsInLevel >= i)
                starsImage[i - 1].sprite = starFilled;
        }

        LevelManager.LevelData().stars = starsWonInLevel;

        SaveLoadSystem.SaveGame();

        /*
        _uiAnimator.StartAnimation(_children, singleDuration, singleDelay, singleSpawnCurve,
            () => { EventSystemScript.ChangeFirstSelected(firstSelected); });
            */

        _uiAnimator.StartAnimation(_children, singleDuration, singleDelay, singleSpawnCurve,
            () => { EventSystemScript.ChangeFirstSelected(_firstSelected); });
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