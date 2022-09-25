using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class CoinScript : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private Transform coin;

    private Transform _whoToFollow;

    //get dotween animation
    private Tweener _tweener;
    public int isFollowing = 0;
    private float _deathTimer;
    private bool _isShrinking = false;

    // Start is called before the first frame update
    private void Start()
    {
        coin.DOLocalRotate(new Vector3(360, 0, 0), 2f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);

        //populate tween
    }

    // Update is called once per frame
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
                transform.DOScale(0, .2f).OnComplete(() => Destroy(gameObject));
            }
        }
    }

    private void OnDestroy()
    {
        LevelManager.coinList.Remove(gameObject);
        PlayerInteractionHandler.OnScoreChange?.Invoke(1);
        DOTween.Kill(coin);
        DOTween.Kill(transform);
    }

    public void Collect(Transform follow)
    {
        if (isFollowing == 0)
        {
            isFollowing = 1;
            _whoToFollow = follow;

            var awayDirection = (transform.position - follow.position).normalized;
            var awayPosition = transform.position + awayDirection * 1.4f;


            transform.DOMove(awayPosition, .25f, false).SetEase(Ease.InOutCubic).onComplete += () =>
            {
                StartFollowingPlayer(follow);
            };
        }
    }

    private void StartFollowingPlayer(Transform follow)
    {
        var tempTransform = transform;
        var tempPosition = tempTransform.position;
        var awayDirection = (tempPosition - follow.position).normalized;
        var position = tempPosition + awayDirection * 2;
        _tweener = transform.DOMove(_whoToFollow.position, .2f, false).SetEase(Ease.OutCubic);
        isFollowing = 2;
        _tweener.Play();
    }
}