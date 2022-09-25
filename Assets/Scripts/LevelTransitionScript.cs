using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelTransitionScript : MonoBehaviour
{
    public static LevelTransitionScript Instance;

    [SerializeField] private RawImage fadeImage;
    private RenderTexture _renderTexture;

    private void StartTransition()
    {
        StartCoroutine(CoroutineScreenshot());
    }

    private void OnEnable()
    {
        LevelManager.StartLevelTransition += StartTransition;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);


        DontDestroyOnLoad(transform.root.gameObject);
    }

    private void OnDisable()
    {
        LevelManager.StartLevelTransition -= StartTransition;
    }


    private IEnumerator CoroutineScreenshot()
    {
        yield return new WaitForEndOfFrame();

        _renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        ScreenCapture.CaptureScreenshotIntoRenderTexture(_renderTexture);

        fadeImage.material.SetTexture("_screenshot", _renderTexture);
        fadeImage.gameObject.SetActive(true);


        DOVirtual.Float(0, 1f, 3.5f, value => fadeImage.material.SetFloat("_maskSize", value)).SetEase(Ease.InCirc)
            .onComplete += () =>
        {
            fadeImage.gameObject.SetActive(false);
            _renderTexture.Release();
        };

        GameManagerScript.LoadNextLevel();
    }
}