using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Reward
{
    public GameObject rewardPrefab;
    public int count;
}

public class RewardScript : MonoBehaviour
{
    [HideInInspector]
    public enum DropType
    {
        Explosion
    }

    public DropType dropType;

    public List<Reward> rewards;
    private List<GameObject> spawnedRewards;
    private Vector3 _position;

    private void Start()
    {
        _position = transform.position;
        spawnedRewards = new List<GameObject>();

        foreach (var reward in rewards)
        foreach (var i in Enumerable.Range(0, reward.count))
        {
            var rewardObject = Instantiate(reward.rewardPrefab, _position, Quaternion.identity);
            spawnedRewards.Add(rewardObject);

            var collectableComponent = rewardObject.GetComponent<CollectableBehaviour>();
            if (collectableComponent != null)
                rewardObject.GetComponent<CollectableBehaviour>().AddToScore();

            rewardObject.SetActive(false);
        }
    }

    public void SpawnReward()
    {
        for (var index = 0; index < spawnedRewards.Count; index++)
        {
            var reward = spawnedRewards[index];
            if (dropType == DropType.Explosion)
            {
                reward.SetActive(true);
                float randomDir2D = Random.Range(0, 360);
                var randomDir3D = new Vector3(Mathf.Cos(randomDir2D), Mathf.Sin(randomDir2D), 0);
                reward.transform.DOMove(_position + randomDir3D * Random.Range(1f, 2.5f), 0.5f).SetAutoKill(true);
            }
        }
    }
}