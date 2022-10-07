using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelTransitionScript : MonoBehaviour
{
    public static LevelTransitionScript Instance;

    [SerializeField] private RawImage transitionImage;
    private RenderTexture _renderTexture;
    private Camera _camera;

    private Tween _transitionImageTween;
    private static readonly int CompareValue = Shader.PropertyToID("_CompareValue");

    private void OnEnable()
    {
        LevelManager.StartLevelTransition += StartTransition;
        _camera = Camera.main;
        transitionImage.material.SetFloat(CompareValue, .55f);
    }

/*
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);


        DontDestroyOnLoad(gameObject);
    }
    */

    private void OnDisable()
    {
        LevelManager.StartLevelTransition -= StartTransition;
    }

    private void StartTransition(Vector3 portalPosition)
    {
        transitionImage.gameObject.SetActive(true);
        if (_camera == null)
            _camera = Camera.main;

        transitionImage.material.SetFloat(CompareValue, 0);
        DOTween.To(() => transitionImage.material.GetFloat(CompareValue),
                x => transitionImage.material.SetFloat(CompareValue, x), 0.55f, 1.5f)
            .SetAutoKill(false).onComplete += () => { LevelManager.LoadNextLevel(); };
    }

    public void ReverseTransition(Vector3 portalPosition)
    {
        if (_camera == null)
            _camera = Camera.main;

        transitionImage.material.SetFloat(CompareValue, .55f);
        DOTween.To(() => transitionImage.material.GetFloat(CompareValue),
                x => transitionImage.material.SetFloat(CompareValue, x), 0f, 1.5f)
            .SetAutoKill(false).onComplete += () => { transitionImage.gameObject.SetActive(false); };
    }


/*
    private IEnumerator LevelTransition(Vector3 portalPosition)
    {
        transitionImage.gameObject.SetActive(true);
        
    }


    private IEnumerator CoroutineScreenshot(Vector3 portalPosition)
    {
        yield return new WaitForEndOfFrame();

        _renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        ScreenCapture.CaptureScreenshotIntoRenderTexture(_renderTexture);

        transitionImage.material.SetTexture(Screenshot, _renderTexture);
        


        if (_camera == null)
            _camera = Camera.main;

        var portalPositionOnScreen = _camera.WorldToViewportPoint(portalPosition);

        transitionImage.material.SetFloat(XPos, portalPositionOnScreen.x);
        transitionImage.material.SetFloat(YPos, portalPositionOnScreen.y);


        transitionImage.material.SetFloat(MaskSize, 0);

        _maskSizeTween = DOTween.To(() => transitionImage.material.GetFloat(MaskSize),
                x => transitionImage.material.SetFloat(MaskSize, x), 40f, 1.5f)
            .SetEase(Ease.InCirc).SetAutoKill(false).Pause();

        _maskSizeTween.onComplete += () =>
        {
            transitionImage.gameObject.SetActive(false);
            _renderTexture.Release();
        };

        _maskSizeTween.onPlay += () =>
        {
            transitionImage.gameObject.SetActive(true);
            GameManagerScript.LoadNextLevel();
        };


        _maskSizeTween.Restart();
    }
    */
}