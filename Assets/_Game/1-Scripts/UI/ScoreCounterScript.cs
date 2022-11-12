using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScoreCounterScript : MonoBehaviour
{
    public TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _scoreMinMax;
    private Tween _scorePunchTween;
    private WaitForEndOfFrame _waitFrame = new();


    private void OnEnable()
    {
        LevelManager.OnScoreChanged += UpdateScoreUI;
    }

    private void OnDisable()
    {
        LevelManager.OnScoreChanged -= UpdateScoreUI;
        _scorePunchTween.Kill();
    }

    private void Start()
    {
        _scoreMinMax.SetText(CurrentLevelHolder.GetCurrentLevel().scoreToWin.ToString() + "\n" +
                             CurrentLevelHolder.GetCurrentLevel().totalScore.ToString());

        _scorePunchTween = _scoreText.transform.DOPunchScale(Vector3.one * .05f, .5f, 1, 1).SetAutoKill(false).Pause();
        _scorePunchTween.onComplete += () => _scoreText.transform.localScale = Vector3.one;
    }

    private void UpdateScoreUI(int score)
    {
        _scorePunchTween.Restart();

        StartCoroutine(UpdateScoreUIRoutine());
    }

    private IEnumerator UpdateScoreUIRoutine()
    {
        yield return _waitFrame;
        _scoreText.SetText(LevelManager.GetScore().ToString());
    }
}