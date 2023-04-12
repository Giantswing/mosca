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

    [SerializeField] private SmartData.SmartEvent.EventDispatcher levelTransitionEnded;
    private bool _isTransitioning;


    [Header("Audio Events")] [SerializeField]
    private AudioEventSO startTransitionAudioEvent;

    [SerializeField] private AudioEventSO endTransitionAudioEvent;

    private void OnEnable()
    {
        _camera = Camera.main;
        transitionImage.material.SetFloat(CompareValue, .55f);
        _isTransitioning = false;
    }


    public void InitTransition()
    {
        if (_isTransitioning) return;

        transitionImage.material.DOKill();
        SoundMaster.PlaySound(transform.position, (int)SoundListAuto.LevelPortalTransitionIn, false);
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
            {
                finishTransition.value = true;
                DOTween.KillAll();
            }
        };
    }

    public void ReverseTransition(Vector3 portalPosition)
    {
        transitionImage.material.DOKill();
        GC.Collect();

        SoundMaster.PlaySound(transform.position, (int)SoundListAuto.LevelPortalTransitionOut, false);

        if (_camera == null)
            _camera = Camera.main;

        transitionImage.material.SetFloat(CompareValue, .55f);
        DOTween.To(() => transitionImage.material.GetFloat(CompareValue),
                x => transitionImage.material.SetFloat(CompareValue, x), 0f, 1f)
            .SetAutoKill(false).onComplete += () =>
        {
            transitionImage.gameObject.SetActive(false);
            levelTransitionEnded.Dispatch();
        };
    }
}