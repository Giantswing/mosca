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

        for (var index = 0; index < rewards.Count; index++)
        {
            Reward reward = rewards[index];

            for (var i = 0; i < reward.count; i++)
            {
                GameObject rewardObject = Instantiate(reward.rewardPrefab, _position, Quaternion.identity);
                rewardObject.transform.parent = transform;
                spawnedRewards.Add(rewardObject);

                if (rewardObject.TryGetComponent(out PickUpEffect_Score score)) score.AddScoreExternally();

                /*        
                CollectableBehaviour collectableComponent = rewardObject.GetComponent<CollectableBehaviour>();
                if (collectableComponent != null)
                    rewardObject.GetComponent<CollectableBehaviour>().AddToScore();
                */


                rewardObject.SetActive(false);
            }
        }
    }

    public void SpawnReward()
    {
        _position = transform.position;
        for (var index = 0; index < spawnedRewards.Count; index++)
        {
            GameObject reward = spawnedRewards[index];
            if (dropType == DropType.Explosion)
            {
                reward.SetActive(true);
                reward.transform.parent = null;
                reward.transform.position = _position;
                float randomDir2D = Random.Range(0, 360);


                Vector3 randomDir3D = new(Mathf.Cos(randomDir2D), Mathf.Sin(randomDir2D), 0);
                reward.transform.DOMove(_position + randomDir3D * Random.Range(2.5F, 3.5F), Random.Range(0.35f, 0.5f));


                PickUpBase pickUp = reward.GetComponent<PickUpBase>();
                Transform closestPlayer = TargetGroupControllerSystem.ClosestPlayer(transform);
                pickUp.whoToFollow = closestPlayer;
                pickUp.whoReceivesPickup = closestPlayer;

                pickUp.StartFollowingWithDelay();
            }
        }
    }
}