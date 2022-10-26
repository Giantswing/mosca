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

    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float fadeDelay = 2f;

    private IDisposable _tempInputSystem;
    private bool _isAndroid;
    private bool _allowSkipIntro;
    [SerializeField] private SmartData.SmartBool.BoolReader showIntro;

    private void Start()
    {
        _allowSkipIntro = true;
        StartIntroScene();

        if (Application.platform == RuntimePlatform.Android)
            _isAndroid = true;


        if (SceneManager.GetSceneByName("_levelSelection").isLoaded || _isAndroid)
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
        if (levelNameText != null && showIntro)
        {
            levelNameText.SetText(LevelManager.LevelData().sceneName);
            levelObjectivesText.SetText(
                "Win <color=red>at least</color> " + LevelManager.LevelData().scoreToWin +
                " points\nBonus time: " + LevelManager.LevelData().timeToWin + " seconds");
            levelNameText.gameObject.SetActive(true);
            levelObjectivesText.gameObject.SetActive(true);
            transitionImage.SetActive(true);

            StartCoroutine(IntroScene());
        }
        else
        {
            transitionImage.SetActive(true);
            levelTransitionScript.ReverseTransition(Vector3.zero);
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

            if (_allowSkipIntro)
                _tempInputSystem.Dispose();
        }
    }
}