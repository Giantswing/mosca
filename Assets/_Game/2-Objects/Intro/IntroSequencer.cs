using System;
using UnityEngine;
using UnityEngine.Video;

public class IntroSequencer : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private SmartData.SmartEvent.EventDispatcher onIntroFinished;
    [SerializeField] private SmartData.SmartInt.IntWriter transitionState;
    [SerializeField] private SmartData.SmartBool.BoolWriter allowAsyncLoad;
    [SerializeField] private SmartData.SmartBool.BoolWriter showIntro;
    [SerializeField] private SmartData.SmartBool.BoolWriter showIntroText;

    private void Awake()
    {
        showIntro.value = false;
        showIntroText.value = false;
    }

    private void OnEnable()
    {
        videoPlayer.loopPointReached += EndReached;
    }

    private void OnDisable()
    {
        videoPlayer.loopPointReached -= EndReached;
    }

    private void EndReached(VideoPlayer vp)
    {
        //transitionState.value = (int)LevelLoader.LevelTransitionState.LoadNextInBuild;
        //onIntroFinished.Dispatch();

        LevelLoadSystem.LoadLevel(LevelLoadSystem.LevelToLoad.LoadNextInBuild);
        Destroy(vp.gameObject);
    }
}