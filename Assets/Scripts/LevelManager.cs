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
    public static List<GameObject> coinList = new();
    private int _coinsToWin;

    private int _score = 0;
    private TextMeshProUGUI _scoreText;
    private Tween _scorePunchTween;

    private GameObject portal;


    // Level and level transition management
    [SerializeField] private LevelRules levelRules;

    public static UnityAction<Vector3> StartLevelTransition;
    public static UnityAction<string> UpdateScoreUI;

    private void OnEnable()
    {
        PlayerInteractionHandler.OnScoreChanged += UpdateScore;
        _score = 0;
        coinList.AddRange(GameObject.FindGameObjectsWithTag("Coin"));
        _coinsToWin = coinList.Count;
        portal = GameObject.FindGameObjectWithTag("Meta");
        portal.SetActive(false);
    }

    private void OnDisable()
    {
        PlayerInteractionHandler.OnScoreChanged -= UpdateScore;
    }

    private void Start()
    {
        UpdateScoreUI?.Invoke(_score.ToString() + "/" + _coinsToWin.ToString());
    }

    private void UpdateScore(int scoreChange)
    {
        _score += scoreChange;
        UpdateScoreUI?.Invoke(_score.ToString() + "/" + _coinsToWin.ToString());
        CheckWin();
    }

    private void CheckWin()
    {
        var transitionLevel = false;

        if (levelRules.winByCoins) transitionLevel = coinList.Count == 0 ? true : false;

        if (transitionLevel) OpenPortal();
    }

    private void OpenPortal()
    {
        portal.SetActive(true);
    }
}