using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

public class LevelIntroScript : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private GameObject transitionImage;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI levelObjectivesText;


    [SerializeField] private LevelTransitionScript levelTransitionScript;

    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float fadeDelay = 0.75f;

    private IDisposable _tempInputSystem;
    private bool _isAndroid;
    private bool _allowSkipIntro;
    [SerializeField] private SmartData.SmartBool.BoolReader showIntro;
    [SerializeField] private SmartData.SmartBool.BoolReader showIntroText;


    private void Start()
    {
        fadeDuration = 0.5f;
        fadeDelay = 2.5f;
        _allowSkipIntro = true;
        StartIntroScene();

        if (Application.platform == RuntimePlatform.Android)
            _isAndroid = true;


        if (SceneManager.GetSceneByName("_levelSelection").isLoaded || _isAndroid)
            _allowSkipIntro = false;

        _allowSkipIntro = false;

        /*

        if (_allowSkipIntro)
            _tempInputSystem = InputSystem.onAnyButtonPress.Call(
                ctrl => { StopIntro(); });
                */
    }


    /*
    private void OnDisable()
    {
        if (_allowSkipIntro)
            if (Application.platform != RuntimePlatform.Android)
                _tempInputSystem.Dispose();
    }
    */


    public void StartIntroScene()
    {
        var currentLevel = CurrentLevelHolder.GetCurrentLevel();
        if (showIntro.value == false) return;

        if (currentLevel != null && levelNameText != null && showIntroText.value == true)
        {
            levelNameText.SetText(currentLevel.sceneName);
            levelObjectivesText.SetText(
                "Win <color=red>at least</color> " + LevelManager._scoreForStars[0] +
                " points\nBonus time: " + currentLevel.timeToWin + " seconds");
            levelNameText.gameObject.SetActive(true);
            levelObjectivesText.gameObject.SetActive(true);
            transitionImage.SetActive(true);

            StartCoroutine(IntroScene());
        }
        else
        {
            transitionImage.SetActive(true);
            StopIntro();
        }
    }

    private IEnumerator IntroScene()
    {
        yield return new WaitForSecondsRealtime(fadeDelay);

        DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, fadeDuration).onComplete =
            () => { StopIntro(); };
    }

    private void StopIntro()
    {
        //print("intro stopped");
        levelTransitionScript.ReverseTransition(Vector3.zero);

        if (levelNameText != null)
        {
            levelNameText.gameObject.SetActive(false);
            levelObjectivesText.gameObject.SetActive(false);

            /*
            if (_allowSkipIntro)
                _tempInputSystem.Dispose();
                */
        }
    }
}