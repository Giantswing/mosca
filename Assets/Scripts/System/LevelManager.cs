using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
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

    public static LevelManager Instance { get; private set; }

    public static UnityAction<Vector3> StartLevelTransition;
    public static UnityAction LevelCompleted;

    [SerializeField] private GameObject winScreen;
    [SerializeField] private PortalPopUpScript portalPopUp;

    public static float LevelTime { get; private set; }

    private void OnEnable()
    {
        OnScoreChanged += UpdateScore;
    }

    private void Awake()
    {
        Instance = this;
        _score = 0;
        ScoreToWin = 0;

        portal = GameObject.FindGameObjectWithTag("Meta");
        portal.SetActive(false);
    }

    private void OnDisable()
    {
        OnScoreChanged -= UpdateScore;
    }

    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 45;
        }

        winScreen.SetActive(true);

        //StartCoroutine(TestCoroutine());
    }


    private IEnumerator TestCoroutine()
    {
        yield return new WaitForSeconds(1f);
        LevelCompleted?.Invoke();
    }


    private void Update()
    {
        LevelTime += Time.deltaTime;
    }

    private void UpdateScore(int scoreChange)
    {
        _score += scoreChange;
        CheckWin();
    }

    private void CheckWin()
    {
        var transitionLevel = _score >= levelData.scoreToWin ? true : false;
        if (transitionLevel)
        {
            portalPopUp.gameObject.SetActive(true);
            portalPopUp.portalTransform = portal.transform;
            //portalPopUp.ShowPopUp();
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        OnScoreChanged?.Invoke(0);
    }

    public static void LoadNextLevel()
    {
        var nextLevelIndex = Instance.campaignData.GetLevelIndex(Instance.levelData);

        SceneManager.LoadScene(Instance.campaignData.level[nextLevelIndex + 1].scene);
        OnScoreChanged?.Invoke(0);
    }

    public static void GoToMenu()
    {
        SceneManager.LoadScene(Instance.campaignData.levelSelectionScene);
    }

    public static void ShowPopUp()
    {
    }

    public static LevelSO LevelData()
    {
        return Instance.levelData;
    }

    public static int GetScore()
    {
        return Instance._score;
    }
}