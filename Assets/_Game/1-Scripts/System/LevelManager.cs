using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using SmartData.SmartEvent;
using TMPro;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private GameObject portal;
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


    public EventDispatcher transitionEvent;
    public SmartData.SmartInt.IntWriter transitionType;

    [SerializeField] private AttributeDataSO playerDataAttributes;
    public static Action OnHeartContainersChanged;

    //public SmartData.SmartInt.IntWriter playerScore;

    public int[] _scoreForStars;
    public SmartData.SmartInt.IntReader currentScore;
    public SmartData.SmartInt.IntReader maxScore;

    private void OnEnable()
    {
        HeartContainersUI.OnHeartFilledAnimationEnd += IncreaseMaxHealth;
    }

    private void OnDisable()
    {
        HeartContainersUI.OnHeartFilledAnimationEnd -= IncreaseMaxHealth;
    }

    private void Awake()
    {
        Instance = this;
        levelData = campaignData.defaultScene;
        campaignData.levels.ForEach(x =>
        {
            string levelName = x.sceneInternalName;
            if (levelName == SceneManager.GetActiveScene().name) levelData = x;
        });
        campaignData.UpdateLevelInfo();
    }

    private void Start()
    {
        _scoreForStars = new int[3];
        SaveLoadSystem.LoadGame();
        portal = GameObject.FindGameObjectWithTag("Meta");

        if (portal != null)
            portal.SetActive(false);

        if (Application.platform == RuntimePlatform.Android)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        winScreen.SetActive(true);

        levelMaxTime.value = levelData.timeToWin;


        DOVirtual.DelayedCall(0.1f, () =>
        {
            SetUpStartingHealth();
            _scoreForStars[2] = maxScore.value;
            _scoreForStars[1] = Mathf.RoundToInt(maxScore.value / 1.5f);
            _scoreForStars[0] = Mathf.RoundToInt(maxScore.value / 3f);
        });
    }

    private void OnApplicationQuit()
    {
        DOTween.KillAll();
    }


    public static LevelSO GetCurrentLevel()
    {
        if (Instance != null)
            return Instance.levelData;
        else
            return null;
    }

    public static void IncreaseHeartContainers(int heartId)
    {
        Instance.campaignData.heartContainers++;
        Instance.campaignData.heartContainerIDs.Add(heartId);
        OnHeartContainersChanged?.Invoke();


        //SaveLoadSystem.SaveGame();
    }


    public static CampaignSO GetCurrentCampaign()
    {
        if (Instance != null)
            return Instance.campaignData;
        else
            return null;
    }


    private void SetUpStartingHealth()
    {
        var currentMaxHealth = 3;
        var currentHeartToAdd = 0;

        currentHeartToAdd = Mathf.FloorToInt(campaignData.heartContainers / 3);

        /*
        playerHealthMax.value = currentMaxHealth + currentHeartToAdd;
        playerHealth.value = playerHealthMax.value;
        */

        if (playerDataAttributes == null) return;
        playerDataAttributes.attributes.maxHP = currentMaxHealth + currentHeartToAdd;
        playerDataAttributes.attributes.HP = playerDataAttributes.attributes.maxHP;
    }

    public static void IncreaseMaxHealth()
    {
        if (Instance.playerDataAttributes == null) return;
        Instance.playerDataAttributes.attributes.maxHP++;
        Instance.playerDataAttributes.attributes.HP = Instance.playerDataAttributes.attributes.maxHP;
    }


    public static List<CheckpointScript> GetCheckpoints()
    {
        return Instance.Checkpoints;
    }

    public static void ReorderCheckpoints()
    {
        Instance.Checkpoints.Sort((x, y) => x.checkpointNumber.CompareTo(y.checkpointNumber));
    }

    public static void ResetLevel()
    {
        Instance.transitionType.value = (int)LevelLoader.LevelTransitionState.Restart;
        Instance.transitionEvent.Dispatch();
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

    public void CheckWin()
    {
        if (_isPortalOpen) return;

        //var transitionLevel = _score >= levelData.scoreToWin ? true : false;
        bool transitionLevel = currentScore.value >= _scoreForStars[0] ? true : false;
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
        return Instance.currentScore.value;
    }
}