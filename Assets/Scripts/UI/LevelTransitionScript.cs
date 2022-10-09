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

    private void OnDisable()
    {
        LevelManager.StartLevelTransition -= StartTransition;
    }

    private void StartTransition(Vector3 portalPosition)
    {
        StartCoroutine(TransitionCoroutine(portalPosition));
    }

    private IEnumerator TransitionCoroutine(Vector3 portalPosition)
    {
        transitionImage.gameObject.SetActive(true);
        if (_camera == null)
            _camera = Camera.main;

        transitionImage.material.SetFloat(CompareValue, 0);
        DOTween.To(() => transitionImage.material.GetFloat(CompareValue),
                x => transitionImage.material.SetFloat(CompareValue, x), 0.55f, 2.5f)
            .SetAutoKill(false);

        yield return new WaitForSeconds(.5f);

        LevelManager.LevelCompleted?.Invoke();
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
}