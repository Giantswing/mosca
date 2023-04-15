using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScoreCounterScript : MonoBehaviour
{
    public TextMeshProUGUI _scoreText;
    public SmartData.SmartInt.IntReader currentScore;

    private void Start()
    {
        UpdateScoreUI();
    }

    public void UpdateScoreUI()
    {
        _scoreText.SetText(currentScore.value.ToString());
    }
}