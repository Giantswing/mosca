using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelReferences : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI scoreText;
    public Volume fxVolume;

    public static LevelReferences instance;

    private void Start()
    {
        instance = this;
    }

    public static TextMeshProUGUI GetFpsText()
    {
        return instance.fpsText;
    }

    public static TextMeshProUGUI GetScoreText()
    {
        return instance.scoreText;
    }

    public static Volume GetFxVolume()
    {
        return instance.fxVolume;
    }
}