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
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void StartTransition(Vector3 portalPosition)
    {
        StartCoroutine(CoroutineScreenshot(portalPosition));
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


    private IEnumerator CoroutineScreenshot(Vector3 portalPosition)
    {
        yield return new WaitForEndOfFrame();

        _renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        ScreenCapture.CaptureScreenshotIntoRenderTexture(_renderTexture);

        fadeImage.material.SetTexture("_screenshot", _renderTexture);
        fadeImage.gameObject.SetActive(true);


        //var portalPositionOnScreen = _camera.WorldToScreenPoint(portalPosition);
        var portalPositionOnScreen = _camera.WorldToViewportPoint(portalPosition);

        fadeImage.material.SetFloat("_XPos", portalPositionOnScreen.x);
        fadeImage.material.SetFloat("_YPos", portalPositionOnScreen.y);


        DOVirtual.Float(0, 1f, 1.5f, value => fadeImage.material.SetFloat("_maskSize", value)).SetEase(Ease.InCirc)
            .onComplete += () =>
        {
            fadeImage.gameObject.SetActive(false);
            _renderTexture.Release();
        };

        GameManagerScript.LoadNextLevel();
    }
}