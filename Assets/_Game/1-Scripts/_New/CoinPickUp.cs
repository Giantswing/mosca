using System;

[Serializable]
public class CoinPickUp : PickUpBase
{
    public int scoreValue = 1;
    private bool hasAddedScore = false;

    protected override void Initialize()
    {
        base.Initialize();
        AddToScore();
    }

    public void AddToScore()
    {
        if (hasAddedScore) return;
        if (LevelManager.Instance != null)
            LevelManager._maxScore += scoreValue;
        hasAddedScore = true;
    }

    public override void EndCollect()
    {
        base.EndCollect();
        LevelManager.OnScoreChanged?.Invoke(scoreValue);
    }
}