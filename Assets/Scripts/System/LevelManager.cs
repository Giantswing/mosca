using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utilities;

public class LevelManager : MonoBehaviour
{
    //CREATE LIST OF COINS
    public static int ScoreToWin = 0;

    private int _score = 0;
    private TextMeshProUGUI _scoreText;
    private Tween _scorePunchTween;

    private GameObject portal;
    public static UnityAction<int> OnScoreChanged;

    [SerializeField] private LevelSO levelData;
    [SerializeField] private CampaignSO campaignData;

    private bool _isPortalOpen = false;

    public enum LevelTransitionState
    {
        Restart,
        NextLevel,
        SpecificLevel
    }

    public static LevelManager Instance { get; private set; }

    public static UnityAction<int, SceneField> StartLevelTransition;
    public static UnityAction<float> UpdateTimer;

    //public static UnityAction LevelCompleted<bool ShowWinScreen, SceneField levelToLoad>;

    [SerializeField] private GameObject winScreen;
    [SerializeField] private PortalPopUpScript portalPopUp;

    public static float LevelTime { get; private set; }
    private float _timeReset = 0f;

    private void OnEnable()
    {
        OnScoreChanged += UpdateScore;
    }

    private void Awake()
    {
        Instance = this;
        _score = 0;
        ScoreToWin = 0;
        var sceneName = "Scenes/" + SceneManager.GetActiveScene().name;
        if (levelData == null) levelData = campaignData.GetCurrentLevel(sceneName);


        SaveLoadSystem.LoadGame();
    }

    private void OnDisable()
    {
        OnScoreChanged -= UpdateScore;
    }

    private void Start()
    {
        portal = GameObject.FindGameObjectWithTag("Meta");
        portal.SetActive(false);

        if (Application.platform == RuntimePlatform.Android)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }


        winScreen.SetActive(true);
        UpdateTimer?.Invoke(LevelTime);
    }


    /*
    private IEnumerator TestCoroutine()
    {
        yield return new WaitForSeconds(1f);
        LevelCompleted?.Invoke();
    }
    */


    private void Update()
    {
        LevelTime += Time.deltaTime;

        _timeReset += Time.deltaTime;
        if (_timeReset >= 1f)
        {
            UpdateTimer?.Invoke(LevelTime);
            _timeReset = 0f;
        }
    }

    private void UpdateScore(int scoreChange)
    {
        _score += scoreChange;
        CheckWin();
    }

    private void CheckWin()
    {
        if (_isPortalOpen) return;

        var transitionLevel = _score >= levelData.scoreToWin ? true : false;
        if (transitionLevel)
        {
            _isPortalOpen = true;
            portalPopUp.gameObject.SetActive(true);
            portalPopUp.portalTransform = portal.transform;
            OpenPortal();
        }
    }

    private void OpenPortal()
    {
        if (portal != null)
            portal.SetActive(true);
    }

    public static void RestartLevel()
    {
        LevelTime = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        OnScoreChanged?.Invoke(0);
    }

    public static void LoadNextLevel()
    {
        var nextLevelIndex = Instance.campaignData.GetLevelIndex(Instance.levelData);

        SceneManager.LoadScene(Instance.campaignData.levels[nextLevelIndex + 1].scene);
        OnScoreChanged?.Invoke(0);
    }

    public static void LoadSpecificLevel(SceneField scene)
    {
        SceneManager.LoadScene(scene);
    }

    public static void GoToMenu()
    {
        SceneManager.LoadScene(Instance.campaignData.levelSelectionScene);
    }


    public static LevelSO LevelData()
    {
        return Instance.levelData;
    }

    public static CampaignSO CampaignData()
    {
        return Instance.campaignData;
    }

    public static int GetScore()
    {
        return Instance._score;
    }


    public static bool isThisLastLevel()
    {
        if (Instance.campaignData.GetLevelIndex(Instance.levelData) == Instance.campaignData.levels.Count - 1)
            return true;
        else
            return false;
    }
}