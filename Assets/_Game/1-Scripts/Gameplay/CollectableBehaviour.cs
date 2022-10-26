using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CollectableBehaviour : MonoBehaviour
{
    private bool _isShrinking = false;
    private Tweener _tweener;

    [HideInInspector] public int isFollowing = 0;
    private Transform _whoToFollow;

    [SerializeField] private Transform displayObject;
    [SerializeField] private Collider myCollider;
    [SerializeField] private SmartData.SmartInt.IntWriter playerHealth;
    [SerializeField] private SmartData.SmartEvent.EventDispatcher onCollect;

    [Space(10)] [SerializeField] private SimpleAudioEvent collectSound;

    //private bool hasAddedScore = false;

    [HideInInspector]
    public enum PickUpAnimation
    {
        Rotatable
    }

    public PickUpAnimation pickUpAnimation;

    [HideInInspector]
    public enum PickUp
    {
        Coin,
        Poop
    }

    public PickUp pickUp;

    public int scoreValue = 1;

/*
    public void AddToScore()
    {
        LevelManager.ScoreToWin += scoreValue;
        hasAddedScore = true;
    }
    */

    private void Start()
    {
        //if (!hasAddedScore) AddToScore();

        switch (pickUpAnimation)
        {
            case PickUpAnimation.Rotatable:
                displayObject.DOLocalRotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
                break;
        }

        myCollider.enabled = false;
        StartCoroutine(AllowCollection());
    }

    private IEnumerator AllowCollection()
    {
        yield return new WaitForSeconds(0.5f);
        myCollider.enabled = true;
    }


    private void Update()
    {
        if (isFollowing == 2)
        {
            var position = _whoToFollow.position;
            _tweener.ChangeEndValue(position, true);
            var distance = Vector3.Distance(transform.position, position);
            if (distance < .6f && _isShrinking == false)
            {
                _isShrinking = true;
                transform.DOScale(0, .1f).OnComplete(() =>
                {
                    GlobalAudioManager.PlaySound(collectSound, transform.position);
                    Destroy(gameObject);
                });
            }
        }
    }

    private void OnDestroy()
    {
        if (pickUp == PickUp.Coin) LevelManager.OnScoreChanged?.Invoke(scoreValue);

        if (pickUp == PickUp.Poop)
        {
            playerHealth.value++;
            onCollect.Dispatch();
        }

        DOTween.Kill(transform);
        DOTween.Kill(displayObject);
    }

    public void Collect(Transform follow)
    {
        if (isFollowing == 0)
        {
            isFollowing = 1;
            _whoToFollow = follow;

            var awayDirection = (transform.position - follow.position).normalized;
            var awayPosition = transform.position + awayDirection * 1.4f;


            transform.DOMove(awayPosition, Random.Range(0.15F, 0.35F), false).SetEase(Ease.InOutCubic).onComplete +=
                () => { StartFollowingPlayer(follow); };
        }
    }

    public void StartFollowingPlayer(Transform follow)
    {
        isFollowing = 1;
        _whoToFollow = follow;

        var tempTransform = transform;
        var tempPosition = tempTransform.position;
        var awayDirection = (tempPosition - follow.position).normalized;
        var position = tempPosition + awayDirection * 2;

        _tweener = transform.DOMove(_whoToFollow.position, .2f, false).SetEase(Ease.OutCubic);
        isFollowing = 2;
        _tweener.Play();
    }
}