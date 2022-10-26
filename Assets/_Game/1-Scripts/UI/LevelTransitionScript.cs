using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelTransitionScript : MonoBehaviour
{
    [SerializeField] private RawImage transitionImage;
    private RenderTexture _renderTexture;
    private Camera _camera;

    private Tween _transitionImageTween;
    private static readonly int CompareValue = Shader.PropertyToID("_CompareValue");
    [SerializeField] private SmartData.SmartBool.BoolWriter finishTransition;
    [SerializeField] private SmartData.SmartInt.IntReader levelTransitionState;
    private bool _isTransitioning;

    private void OnEnable()
    {
        _camera = Camera.main;
        transitionImage.material.SetFloat(CompareValue, .55f);
        _isTransitioning = false;
    }


    public void InitTransition()
    {
        if (_isTransitioning) return;

        _isTransitioning = true;
        transitionImage.gameObject.SetActive(true);
        if (_camera == null)
            _camera = Camera.main;

        transitionImage.material.SetFloat(CompareValue, 0);
        DOTween.To(() => transitionImage.material.GetFloat(CompareValue),
                x => transitionImage.material.SetFloat(CompareValue, x), 0.55f, 1f)
            .SetAutoKill(false).onComplete += () =>
        {
            if (levelTransitionState.value != (int)LevelLoader.LevelTransitionState.DontLoadYet)
                finishTransition.value = true;
        };
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