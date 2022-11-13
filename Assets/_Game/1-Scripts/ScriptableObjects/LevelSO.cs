using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Rules", menuName = "Mosca/Level", order = 1)]
public class LevelSO : ScriptableObject
{
    public SceneField scene;
    public string sceneName;

    [Space(25)] public int scoreToWin;
    public int totalScore;
    public float timeToWin;

    [Space(25)] public int deathCounter = 0;

    [Space(15)] public int stars;
    //public float[] scoreToStars;

    [HideInInspector] public bool isThisLastOne;
    [HideInInspector] public int index;

    [Header("B-Side")] [Space(5)] public bool isBSide;
    public LevelSO bSideScene;


    public void CountScore()
    {
        var collectibles = FindObjectsOfType<CollectableBehaviour>();
        var rewardContainers = FindObjectsOfType<RewardScript>();

        totalScore = 0;

        foreach (var collectible in collectibles) totalScore += collectible.scoreValue;

        foreach (var rewardContainer in rewardContainers)
        foreach (var reward in rewardContainer.rewards)
        {
            var rewardScore = reward.rewardPrefab.GetComponent<CollectableBehaviour>();
            if (rewardScore != null) totalScore += rewardScore.scoreValue * reward.count;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        EditorUtility.SetDirty(this);
    }
#endif
}