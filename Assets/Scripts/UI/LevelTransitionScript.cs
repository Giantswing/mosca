using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Utilities;

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

    private void OnDisable()
    {
        LevelManager.StartLevelTransition -= StartTransition;
    }

    private void StartTransition(int WinState, SceneField levelToLoad)
    {
        transitionImage.gameObject.SetActive(true);
        if (_camera == null)
            _camera = Camera.main;

        transitionImage.material.SetFloat(CompareValue, 0);
        DOTween.To(() => transitionImage.material.GetFloat(CompareValue),
                x => transitionImage.material.SetFloat(CompareValue, x), 0.55f, 1f)
            .SetAutoKill(false).onComplete += () =>
        {
            if (WinState == (int)LevelManager.LevelTransitionState.SpecificLevel)
                LevelManager.LoadSpecificLevel(levelToLoad);
            if (WinState == (int)LevelManager.LevelTransitionState.Restart)
                LevelManager.RestartLevel();
        };
        /*
        yield return new WaitForSeconds(.5f);

        LevelManager.LevelCompleted?.Invoke();
        */
    }

    public void ReverseTransition(Vector3 portalPosition)
    {
        GC.Collect();

        if (_camera == null)
            _camera = Camera.main;

        transitionImage.material.SetFloat(CompareValue, .55f);
        DOTween.To(() => transitionImage.material.GetFloat(CompareValue),
                x => transitionImage.material.SetFloat(CompareValue, x), 0f, 1.5f)
            .SetAutoKill(false).onComplete += () => { transitionImage.gameObject.SetActive(false); };
    }
}