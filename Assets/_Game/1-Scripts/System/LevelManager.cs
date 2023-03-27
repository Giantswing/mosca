using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static int _maxScore = 0;
    public static int _score = 0;
    public static int[] _scoreForStars = new int[3];


    private TextMeshProUGUI _scoreText;
    private Tween _scorePunchTween;

    private GameObject portal;
    public static UnityAction<int> OnScoreChanged;
    public static UnityAction StarsChanged;

    [SerializeField] private LevelSO levelData;
    [SerializeField] private CampaignSO campaignData;

    private AsyncOperation _asyncLoad;

    private bool _isPortalOpen = false;

    public List<CheckpointScript> Checkpoints = new();

    public static LevelManager Instance { get; private set; }

    //public static UnityAction LevelCompleted<bool ShowWinScreen, SceneField levelToLoad>;

    [SerializeField] private GameObject winScreen;
    [SerializeField] private PortalPopUpScript portalPopUp;


    [Header("Time info")] [SerializeField] private SmartData.SmartInt.IntWriter levelTimeInt;
    [SerializeField] private SmartData.SmartFloat.FloatWriter levelTimeFloat;
    [SerializeField] private SmartData.SmartFloat.FloatWriter levelMaxTime;
    private float _timeReset = 0f;


    private void OnEnable()
    {
        OnScoreChanged += UpdateScore;
    }

    public static LevelSO GetCurrentLevel()
    {
        if (Instance != null)
            return Instance.levelData;
        else
            return null;
    }

    private void Awake()
    {
        Instance = this;
        _score = 0;
        _maxScore = 0;

        /*
        if (levelData == null) levelData = CurrentLevelHolder.GetCurrentLevel();
        campaignData.UpdateLevelInfo();
        */

        levelData = campaignData.defaultScene;
        campaignData.levels.ForEach(x =>
        {
            var levelName = x.scene.EditorSceneAsset.name;
            if (levelName == SceneManager.GetActiveScene().name) levelData = x;
        });

        campaignData.UpdateLevelInfo();
        SaveLoadSystem.LoadGame();
    }


    private void SetUpStars()
    {
        _scoreForStars[2] = _maxScore;
        _scoreForStars[1] = Mathf.RoundToInt(_maxScore / 1.5f);
        _scoreForStars[0] = Mathf.RoundToInt(_maxScore / 3f);

        StarsChanged?.Invoke();
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

        levelMaxTime.value = levelData.timeToWin;

        SetUpStars();
    }

    public static List<CheckpointScript> GetCheckpoints()
    {
        return Instance.Checkpoints;
    }

    public static void ReorderCheckpoints()
    {
        Instance.Checkpoints.Sort((x, y) => x.checkpointNumber.CompareTo(y.checkpointNumber));
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
        levelTimeFloat.value += Time.deltaTime;
        _timeReset += Time.deltaTime;

        if (_timeReset >= 1f)
        {
            levelTimeInt.value += 1;
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

        //var transitionLevel = _score >= levelData.scoreToWin ? true : false;
        var transitionLevel = _score >= _scoreForStars[0] ? true : false;
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

    public static int GetScore()
    {
        return _score;
    }
}