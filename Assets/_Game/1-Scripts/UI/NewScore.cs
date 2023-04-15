using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewScore : MonoBehaviour
{
    [SerializeField] private Texture[] starInputTextures;
    [SerializeField] private TextMeshProUGUI[] starTexts = new TextMeshProUGUI[3];
    [SerializeField] private RawImage[] starImages = new RawImage[3];
    [SerializeField] private Image scoreBarMaterial;

    private int[] _scoreForStars;
    private int _currentStars;
    [Space(25)] [SerializeField] private GameObject starPrefab;


    [Space(40)] [Header("Debugging")] [SerializeField]
    private float offsetMult = 1f;

    private static readonly int Score = Shader.PropertyToID("_Score");

    public SmartData.SmartInt.IntReader currentScore;
    public SmartData.SmartInt.IntReader maxScore;

    private void Start()
    {
        _scoreForStars = new int[3];
        DOVirtual.DelayedCall(.1f, SetUpStars);
    }

    private void SetUpStars()
    {
        _scoreForStars[2] = maxScore.value;
        _scoreForStars[1] = Mathf.RoundToInt(maxScore.value / 1.5f);
        _scoreForStars[0] = Mathf.RoundToInt(maxScore.value / 3f);

        for (var i = 0; i < 3; i++)
        {
            GameObject star = Instantiate(starPrefab, transform);
            starTexts[i] = star.GetComponentInChildren<TextMeshProUGUI>();
            starImages[i] = star.GetComponentInChildren<RawImage>();

            RectTransform starTransform = star.GetComponent<RectTransform>();
            float offset = Mathf.Round(_scoreForStars[i]) / _scoreForStars[2];

            starTransform.anchoredPosition = new Vector2(-5f, offset * offsetMult);
        }


        starTexts[0].SetText(_scoreForStars[0].ToString());
        starTexts[1].SetText(_scoreForStars[1].ToString());
        starTexts[2].SetText(_scoreForStars[2].ToString());

        UpdateScore();
    }


    public void UpdateScore()
    {
        scoreBarMaterial.material.SetFloat(Score, currentScore.value / (float)_scoreForStars[2]);

        for (var i = 0; i < 3; i++)
            if (currentScore.value >= _scoreForStars[i])
                //starImages[i].texture = _currentStars > i ? starInputTextures[1] : starInputTextures[2];
                starImages[i].texture = starInputTextures[2];
            else
                starImages[i].texture = _currentStars > i ? starInputTextures[1] : starInputTextures[0];
    }
}