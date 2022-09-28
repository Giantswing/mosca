using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    //CREATE LIST OF COINS
    public static int ScoreToWin = 0;

    private int _score = 0;
    private TextMeshProUGUI _scoreText;
    private Tween _scorePunchTween;

    private GameObject portal;
    public static UnityAction<int> OnScoreChanged;


    // Level and level transition management
    [SerializeField] private LevelRules levelRules;

    public static UnityAction<Vector3> StartLevelTransition;
    public static UnityAction<string> UpdateScoreUI;

    private void OnEnable()
    {
        OnScoreChanged += UpdateScore;
        _score = 0;
        ScoreToWin = 0;

        /*
        var rewards = GameObject.FindGameObjectsWithTag("Collectable");
        foreach (var reward in rewards)
        {
            var rewardComponent = reward.GetComponent<CollectableBehaviour>();
            _scoreToWin += rewardComponent.scoreValue;
        }
        */

        portal = GameObject.FindGameObjectWithTag("Meta");
        portal.SetActive(false);
    }

    private void OnDisable()
    {
        OnScoreChanged -= UpdateScore;
    }

    private void Start()
    {
        StartCoroutine(StartScore());
    }

    private IEnumerator StartScore()
    {
        yield return new WaitForSecondsRealtime(.2f);
        UpdateScoreUI?.Invoke(_score.ToString() + "/" + ScoreToWin.ToString());
        LevelIntroScript.SetLevelRules(levelRules);
    }

    private void UpdateScore(int scoreChange)
    {
        _score += scoreChange;

        UpdateScoreUI?.Invoke(_score.ToString() + "/" + ScoreToWin.ToString());
        CheckWin();
    }

    private void CheckWin()
    {
        var transitionLevel = false;

        if (levelRules.winByScore) transitionLevel = _score == ScoreToWin ? true : false;

        if (transitionLevel) OpenPortal();
    }

    private void OpenPortal()
    {
        if (portal != null)
            portal.SetActive(true);
    }
}