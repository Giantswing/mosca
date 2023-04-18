using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private CampaignSO campaignData;
    private AsyncOperation _asyncLoad;

    [SerializeField] private SmartData.SmartInt.IntWriter instanceLevelTransitionState;
    [SerializeField] private SmartData.SmartBool.BoolWriter instanceAsyncLoadAllowLoad;
    [SerializeField] private SmartData.SmartBool.BoolWriter showLevelIntro;
    [SerializeField] private SmartData.SmartBool.BoolWriter showLevelIntroText;

    public static SceneField SceneToLoad;
    private bool _sceneLoaded = false;

    public static LevelLoader Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


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

    public static void LoadLevel(SceneField sceneToLoad, int transitionState)
    {
        SceneToLoad = sceneToLoad;
        Instance.instanceLevelTransitionState.value = transitionState;
        Instance.LoadLevelInterface();
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
                int nextLevelIndex = LevelManager.GetCurrentLevel().index;
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

    private IEnumerator AsyncLoading()
    {
        while (!_asyncLoad.isDone) yield return null;

        instanceAsyncLoadAllowLoad.value = true;
    }

    private void RestartLevel()
    {
        _asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    private IEnumerator LoadSceneAsync()
    {
        _asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        instanceAsyncLoadAllowLoad.value = false;
        while (!_asyncLoad.isDone && !_sceneLoaded) yield return null;

        instanceAsyncLoadAllowLoad.value = true;
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