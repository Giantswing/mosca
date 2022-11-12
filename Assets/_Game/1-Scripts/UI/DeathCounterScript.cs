using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathCounterScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI deathCounterText;
    public static DeathCounterScript Instance;

    private void Start()
    {
        Instance = this;
        UpdateDeathCounter();
    }

    public static void UpdateDeathCounter()
    {
        Instance.deathCounterText.text = CurrentLevelHolder.GetCurrentLevel().deathCounter.ToString();
    }
}