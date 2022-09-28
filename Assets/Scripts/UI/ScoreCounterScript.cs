using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScoreCounterScript : MonoBehaviour
{
    public TextMeshProUGUI _scoreText;
    private Tween _scorePunchTween;


    private void OnEnable()
    {
        LevelManager.UpdateScoreUI += UpdateScoreUI;
        _scoreText = GetComponent<TextMeshProUGUI>();
    }

    private void OnDisable()
    {
        LevelManager.UpdateScoreUI -= UpdateScoreUI;
        _scorePunchTween.Kill();
    }

    private void UpdateScoreUI(string scoreText)
    {
        _scorePunchTween = _scoreText.transform.DOPunchScale(Vector3.one * .05f, .5f, 1, 1).SetAutoKill(false).Pause();
        _scorePunchTween.onComplete += () => _scoreText.transform.localScale = Vector3.one;
        _scorePunchTween.Restart();

        _scoreText.SetText(scoreText);
    }
}