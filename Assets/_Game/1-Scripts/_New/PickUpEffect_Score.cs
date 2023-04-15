using System;
using UnityEngine;

[Serializable]
public class PickUpEffect_Score : MonoBehaviour, IPickUpEffect
{
    public int scoreValue = 1;

    public SmartData.SmartInt.IntWriter currentScore;
    public SmartData.SmartInt.IntWriter maxScore;
    public SmartData.SmartEvent.EventDispatcher OnScoreChanged;

    private bool hasAddedScore = false;

    private void Start()
    {
        AddScoreExternally();
    }

    public void AddScoreExternally()
    {
        if (!hasAddedScore)
        {
            maxScore.value += scoreValue;
            hasAddedScore = true;
        }
    }

    public void OnCollect(Transform target)
    {
        currentScore.value += scoreValue;
        OnScoreChanged.Dispatch();
    }
}