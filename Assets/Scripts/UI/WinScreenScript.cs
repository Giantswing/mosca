using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

public class WinScreenScript : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected;
    [SerializeField] private UIAnimator _uiAnimator;

    [SerializeField] private TextMeshProUGUI levelNameText, scoreText, timeText;
    [SerializeField] private RawImage winScreenImage, winScreenBG;
    [SerializeField] private RawImage[] starsImage;
    [SerializeField] private Sprite starFilled, starEmpty;
    [SerializeField] private RectTransform scoreParent, timeParent, starsParent;

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
    [SerializeField] private Button[] _buttons;

    private void OnEnable()
    {
        LevelManager.LevelCompleted += StartWinScreenAnimation;
    }

    private void OnDisable()
    {
        LevelManager.LevelCompleted -= StartWinScreenAnimation;
    }

    private void Start()
    {
        _children = GetComponentsInChildren<RectTransform>();

        for (var i = 0; i < _children.Length; i++) _children[i].gameObject.SetActive(false);

        gameObject.SetActive(true);
    }

    private void StartWinScreenAnimation()
    {
        GetComponent<PlayerInput>().enabled = true;
        levelNameText.SetText(LevelManager.LevelData().sceneName);
        scoreText.SetText(LevelManager.GetScore().ToString() + "/" + LevelManager.LevelData().totalScore);
        timeText.SetText(LevelManager.LevelTime.ToString("F1") + "s/" + LevelManager.LevelData().timeToWin + "s");

        _uiAnimator.StartAnimation(_children, singleDuration, singleDelay, singleSpawnCurve,
            () => { EventSystemScript.ChangeFirstSelected(firstSelected); });
    }

    private void HideWinScreenAnimation(string menuAction)
    {
        _uiAnimator.ReverseAnimation(_children, singleHideDuration, singleHideDelay, Ease.OutQuad, () =>
        {
            switch (menuAction)
            {
                case "restart_level":
                    DOTween.KillAll();
                    LevelManager.RestartLevel();
                    break;
                case "go_to_menu":
                    DOTween.KillAll();
                    LevelManager.GoToMenu();
                    break;
                case "next_level":
                    DOTween.KillAll();
                    LevelManager.LoadNextLevel();
                    break;
            }
        });
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