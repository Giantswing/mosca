using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private LevelSO levelData;
    [SerializeField] private CampaignSO campaignData;
    private AsyncOperation _asyncLoad;

    [SerializeField] private SmartData.SmartInt.IntReader instanceLevelTransitionState;
    [SerializeField] private SmartData.SmartBool.BoolWriter instanceAsyncLoadAllowLoad;
    [SerializeField] private SmartData.SmartBool.BoolWriter showLevelIntro;
    public static SceneField SceneToLoad;


    public static LevelLoader Instance { get; private set; }


    public enum LevelTransitionState
    {
        Restart,
        NextLevel,
        SpecificLevel,
        DontLoadYet
    }

    private void OnEnable()
    {
        Instance = this;
        instanceAsyncLoadAllowLoad.BindListener(ChangeAsyncStatus, false);
    }

    public void ChangeAsyncStatus(bool status)
    {
        if (_asyncLoad != null)
        {
            print("Level loader: changing async status to " + status);
            _asyncLoad.allowSceneActivation = status;
        }
    }

    public void LoadLevelInterface()
    {
        switch (instanceLevelTransitionState.value)
        {
            case (int)LevelTransitionState.Restart:
                showLevelIntro.value = false;
                StartCoroutine(LoadSceneAsync());
                break;
            case (int)LevelTransitionState.NextLevel:
                showLevelIntro.value = true;
                var nextLevelIndex = Instance.campaignData.GetLevelIndex(Instance.levelData);
                SceneToLoad = campaignData.levels[nextLevelIndex + 1].scene;
                StartCoroutine(LoadSceneAsync(SceneToLoad));
                break;
            case (int)LevelTransitionState.SpecificLevel:
                showLevelIntro.value = true;
                print("Level loader: loading " + SceneToLoad.EditorSceneAsset.name);
                StartCoroutine(LoadSceneAsync(SceneToLoad));
                break;
            case (int)LevelTransitionState.DontLoadYet:
                break;
        }
    }

    private IEnumerator LoadSceneAsync()
    {
        _asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        instanceAsyncLoadAllowLoad.value = false;
        while (!_asyncLoad.isDone) yield return null;
    }

    private IEnumerator LoadSceneAsync(SceneField scene)
    {
        _asyncLoad = SceneManager.LoadSceneAsync(scene.BuildIndex, LoadSceneMode.Single);
        instanceAsyncLoadAllowLoad.value = false;
        while (!_asyncLoad.isDone) yield return null;
    }

    public static bool IsThisLastLevel()
    {
        if (Instance.campaignData.GetLevelIndex(Instance.levelData) == Instance.campaignData.levels.Count - 1)
            return true;
        else
            return false;
    }
}