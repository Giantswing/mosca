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

    public static LevelTransitionScript Instance { get; private set; }

    public enum TransitionState
    {
        InvisibleAndNotStarted,
        AppearingButNotFinished,
        VisibleAndFinished,
        DisappearingButNotFinished,
        InvisibleAndFinished
    }

    public TransitionState myState;

    private void OnEnable()
    {
        _camera = Camera.main;
        transitionImage.material.SetFloat(CompareValue, .55f);
        _isTransitioning = false;
        Instance = this;
    }

    private void Start()
    {
        ReverseTransition();
    }

    public static void StartTransition(Action callback = null)
    {
        if (Instance._isTransitioning) return;

        if (Instance.myState == TransitionState.VisibleAndFinished)
        {
            callback?.Invoke();
            return;
        }

        Instance.myState = TransitionState.AppearingButNotFinished;

        Instance.transitionImage.material.DOKill();
        SoundMaster.PlaySound(Instance.transform.position, (int)SoundListAuto.LevelPortalTransitionIn, false);
        Instance._isTransitioning = true;
        Instance.transitionImage.gameObject.SetActive(true);
        if (Instance._camera == null)
            Instance._camera = Camera.main;

        GC.Collect();

        Instance.transitionImage.material.SetFloat(CompareValue, 0);
        DOTween.To(() => Instance.transitionImage.material.GetFloat(CompareValue),
                x => Instance.transitionImage.material.SetFloat(CompareValue, x), 0.55f, .75f)
            .SetAutoKill(false).onComplete += () =>
        {
            Instance.myState = TransitionState.VisibleAndFinished;
            callback?.Invoke();
        };
    }

    public void ReverseTransition(Vector3 portalPosition = default)
    {
        transitionImage.material.DOKill();
        myState = TransitionState.DisappearingButNotFinished;

        SoundMaster.PlaySound(transform.position, (int)SoundListAuto.LevelPortalTransitionOut, false);

        if (_camera == null)
            _camera = Camera.main;

        transitionImage.material.SetFloat(CompareValue, .55f);
        DOTween.To(() => transitionImage.material.GetFloat(CompareValue),
                x => transitionImage.material.SetFloat(CompareValue, x), 0f, .75f)
            .SetAutoKill(false).onComplete += () =>
        {
            myState = TransitionState.InvisibleAndFinished;
            transitionImage.gameObject.SetActive(false);
            levelTransitionEnded.Dispatch();
        };
    }
}