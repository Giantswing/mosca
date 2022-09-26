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
        _scorePunchTween = _scoreText.transform.DOPunchScale(Vector3.one * .5f, .5f, 1, 1);
    }

    private void OnDisable()
    {
        LevelManager.UpdateScoreUI -= UpdateScoreUI;
        DOTween.Kill(_scoreText.transform);
    }

    private void UpdateScoreUI(string scoreText)
    {
        _scorePunchTween.Play();

        _scoreText.transform.DOPunchScale(Vector3.one * .1f, .2f).onComplete += () =>
        {
            _scoreText.transform.localScale = Vector3.one;
        };

        _scoreText.SetText(scoreText);
    }
}