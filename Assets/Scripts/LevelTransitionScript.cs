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
    private Tween _maskSizeTween;
    private Tween _testTween;
    private static readonly int MaskSize = Shader.PropertyToID("_maskSize");
    private static readonly int XPos = Shader.PropertyToID("_XPos");
    private static readonly int YPos = Shader.PropertyToID("_YPos");
    private static readonly int Screenshot = Shader.PropertyToID("_screenshot");


    private void StartTransition(Vector3 portalPosition)
    {
        StartCoroutine(CoroutineScreenshot(portalPosition));
    }

    private void OnEnable()
    {
        LevelManager.StartLevelTransition += StartTransition;

        _camera = Camera.main;
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

        fadeImage.material.SetTexture(Screenshot, _renderTexture);
        fadeImage.gameObject.SetActive(true);


        if (_camera == null)
            _camera = Camera.main;

        var portalPositionOnScreen = _camera.WorldToViewportPoint(portalPosition);

        fadeImage.material.SetFloat(XPos, portalPositionOnScreen.x);
        fadeImage.material.SetFloat(YPos, portalPositionOnScreen.y);


        fadeImage.material.SetFloat(MaskSize, 0);

        _maskSizeTween = DOTween.To(() => fadeImage.material.GetFloat(MaskSize),
                x => fadeImage.material.SetFloat(MaskSize, x), 40f, 1.5f)
            .SetEase(Ease.InCirc).SetAutoKill(false).Pause();

        _maskSizeTween.onComplete += () =>
        {
            fadeImage.gameObject.SetActive(false);
            _renderTexture.Release();
        };

        _maskSizeTween.onPlay += () =>
        {
            fadeImage.gameObject.SetActive(true);
            GameManagerScript.LoadNextLevel();
        };


        _maskSizeTween.Restart();
    }
}