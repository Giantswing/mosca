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
    public TextMeshProUGUI scoreText;
    private Tween _scorePunchTween;

    private GameObject portal;


    // Level and level transition management
    [SerializeField] private LevelRules levelRules;

    public static UnityAction StartLevelTransition;

    private void OnEnable()
    {
        PlayerInteractionHandler.OnScoreChanged += UpdateScore;
    }

    private void Start()
    {
        _score = 0;
        coinList.AddRange(GameObject.FindGameObjectsWithTag("Coin"));
        _scorePunchTween = scoreText.transform.DOPunchScale(Vector3.one * .5f, .5f, 1, 1);
        _coinsToWin = coinList.Count;
        portal = GameObject.FindGameObjectWithTag("Meta");
        portal.SetActive(false);
    }

    private void UpdateScore(int scoreChange)
    {
        _score += scoreChange;
        _scorePunchTween.Play();


        scoreText.transform.DOPunchScale(Vector3.one * .1f, .2f).onComplete += () =>
        {
            scoreText.transform.localScale = Vector3.one;
        };


        scoreText.SetText(_score.ToString() + "/" + _coinsToWin.ToString());

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