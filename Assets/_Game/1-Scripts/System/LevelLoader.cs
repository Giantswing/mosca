using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private CampaignSO campaignData;
    private AsyncOperation _asyncLoad;

    [SerializeField] private SmartData.SmartInt.IntReader instanceLevelTransitionState;
    [SerializeField] private SmartData.SmartBool.BoolWriter instanceAsyncLoadAllowLoad;
    [SerializeField] private SmartData.SmartBool.BoolWriter showLevelIntro;
    [SerializeField] private SmartData.SmartBool.BoolWriter showLevelIntroText;

    public static SceneField SceneToLoad;
    private bool _sceneLoaded = false;


    public enum LevelTransitionState
    {
        Restart,
        NextLevel,
        SpecificLevel,
        GoToMenu,
        LoadNextInBuild,
        DontLoadYet
    }

    private void OnEnable()
    {
        instanceAsyncLoadAllowLoad.BindListener(ChangeAsyncStatus, false);
    }

    public void ChangeAsyncStatus(bool status)
    {
        if (_asyncLoad != null)
            // print("Level loader: changing async status to " + status);
            _asyncLoad.allowSceneActivation = status;
    }

    public void LoadLevelInterface()
    {
        switch (instanceLevelTransitionState.value)
        {
            case (int)LevelTransitionState.Restart:
                showLevelIntro.value = true;
                showLevelIntroText.value = false;
                StartCoroutine(LoadSceneAsync());
                break;
            case (int)LevelTransitionState.NextLevel:
                showLevelIntro.value = true;
                showLevelIntroText.value = true;
                var nextLevelIndex = CurrentLevelHolder.GetCurrentLevel().index;
                SceneToLoad = campaignData.levels[nextLevelIndex + 1].scene;
                StartCoroutine(LoadSceneAsync(SceneToLoad));
                break;
            case (int)LevelTransitionState.SpecificLevel:
                showLevelIntro.value = true;
                showLevelIntroText.value = true;
                StartCoroutine(LoadSceneAsync(SceneToLoad));
                break;
            case (int)LevelTransitionState.DontLoadYet:
                break;
            case (int)LevelTransitionState.GoToMenu:
                SceneToLoad = campaignData.levelSelectionScene;
                StartCoroutine(LoadSceneAsync(SceneToLoad));
                break;
            case (int)LevelTransitionState.LoadNextInBuild:
                StartCoroutine(LoadSceneAsync(true));
                break;
        }
    }

    private IEnumerator LoadSceneAsync()
    {
        _asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        instanceAsyncLoadAllowLoad.value = false;
        while (!_asyncLoad.isDone && !_sceneLoaded) yield return null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _sceneLoaded = true;
    }

    private IEnumerator LoadSceneAsync(bool showIntro)
    {
        showLevelIntro.value = showIntro;
        showLevelIntroText.value = showIntro;
        _asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
        instanceAsyncLoadAllowLoad.value = false;
        while (!_asyncLoad.isDone) yield return null;
    }

    private IEnumerator LoadSceneAsync(SceneField scene)
    {
        _asyncLoad = SceneManager.LoadSceneAsync(scene.BuildIndex, LoadSceneMode.Single);
        instanceAsyncLoadAllowLoad.value = false;
        while (!_asyncLoad.isDone) yield return null;
    }
}