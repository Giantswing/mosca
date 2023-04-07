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

    private void Awake()
    {
        _position = transform.position;
        spawnedRewards = new List<GameObject>();

        for (var index = 0; index < rewards.Count; index++)
        {
            var reward = rewards[index];

            for (var i = 0; i < reward.count; i++)
            {
                var rewardObject = Instantiate(reward.rewardPrefab, _position, Quaternion.identity);
                rewardObject.transform.parent = transform;
                spawnedRewards.Add(rewardObject);


                var collectableComponent = rewardObject.GetComponent<CollectableBehaviour>();
                if (collectableComponent != null)
                    rewardObject.GetComponent<CollectableBehaviour>().AddToScore();


                rewardObject.SetActive(false);
            }
        }
    }

    public void SpawnReward()
    {
        _position = transform.position;
        for (var index = 0; index < spawnedRewards.Count; index++)
        {
            var reward = spawnedRewards[index];
            if (dropType == DropType.Explosion)
            {
                reward.SetActive(true);
                reward.transform.parent = null;
                reward.transform.position = _position;
                float randomDir2D = Random.Range(0, 360);
                var randomDir3D = new Vector3(Mathf.Cos(randomDir2D), Mathf.Sin(randomDir2D), 0);
                reward.transform.DOMove(_position + randomDir3D * Random.Range(1.5F, 2.5F), 0.5f).SetAutoKill(true)
                    .onComplete += () =>
                {
                    reward.GetComponent<CollectableBehaviour>()
                        .StartFollowingPlayer(PlayerMovement.ReturnPlayerTransform(),
                            PlayerInteractionHandler.ReturnCrownTransform());
                };

                //print(PlayerMovement.ReturnPlayerTransform());
            }
        }
    }
}