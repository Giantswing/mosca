using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewScore : MonoBehaviour
{
    [SerializeField] private Texture[] starInputTextures;
    [SerializeField] private TextMeshProUGUI[] starTexts = new TextMeshProUGUI[3];
    [SerializeField] private RawImage[] starImages = new RawImage[3];
    [SerializeField] private Image scoreBarMaterial;
    private int _currentStars;
    [Space(25)] [SerializeField] private GameObject starPrefab;


    [Space(40)] [Header("Debugging")] [SerializeField]
    private float offsetMult = 1f;

    private static readonly int Score = Shader.PropertyToID("_Score");

    private void SetUpStars()
    {
        for (var i = 0; i < 3; i++)
        {
            var star = Instantiate(starPrefab, transform);
            starTexts[i] = star.GetComponentInChildren<TextMeshProUGUI>();
            starImages[i] = star.GetComponentInChildren<RawImage>();

            var starTransform = star.GetComponent<RectTransform>();
            var offset = Mathf.Round(LevelManager._scoreForStars[i]) / LevelManager._scoreForStars[2];

            starTransform.anchoredPosition = new Vector2(-5f, offset * offsetMult);
        }


        starTexts[0].SetText(LevelManager._scoreForStars[0].ToString());
        starTexts[1].SetText(LevelManager._scoreForStars[1].ToString());
        starTexts[2].SetText(LevelManager._scoreForStars[2].ToString());

        _currentStars = LevelManager.GetCurrentLevel().stars;
    }

    private void OnEnable()
    {
        LevelManager.OnScoreChanged += UpdateScore;
        LevelManager.StarsChanged += SetUpStars;
    }

    private void OnDisable()
    {
        LevelManager.OnScoreChanged -= UpdateScore;
        LevelManager.StarsChanged -= SetUpStars;
    }

    private void Start()
    {
        UpdateScore(0);
    }

    private void UpdateScore(int newScore)
    {
        scoreBarMaterial.material.SetFloat(Score, LevelManager._score / (float)LevelManager._scoreForStars[2]);

        for (var i = 0; i < 3; i++)
            if (LevelManager._score >= LevelManager._scoreForStars[i])
                //starImages[i].texture = _currentStars > i ? starInputTextures[1] : starInputTextures[2];
                starImages[i].texture = starInputTextures[2];
            else
                starImages[i].texture = _currentStars > i ? starInputTextures[1] : starInputTextures[0];
    }
}