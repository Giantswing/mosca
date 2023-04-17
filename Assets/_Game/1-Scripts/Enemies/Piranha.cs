using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Piranha : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpSpeed = 1f;
    [SerializeField] private float jumpStopTime = 1f;
    [SerializeField] private float waterHeight = 1f;
    [SerializeField] private Transform my3dModel;
    [SerializeField] private float startingTimeOffset = 0f;

    [Space(25)] [SerializeField] private ParticleSystem splashParticles;
    [SerializeField] private int hasSplashed = 1;

    [Space(35)] [SerializeField] private AudioSource audioSource;
    [SerializeField] private SimpleAudioEvent splashOutSound;
    [SerializeField] private SimpleAudioEvent splashInSound;


    private WaitForSeconds _waitUntilJumpStopTime;
    private WaitForSeconds _waitUntilJumpStopTimeHalf;
    private WaitForSeconds _waitStartingTime;
    private Vector3 _startPos;

    private void Start()
    {
        _startPos = transform.position;
        _waitUntilJumpStopTime = new WaitForSeconds(jumpStopTime);
        _waitUntilJumpStopTimeHalf = new WaitForSeconds(jumpStopTime * 0.5f);
        _waitStartingTime = new WaitForSeconds(startingTimeOffset);
        if (startingTimeOffset > 0)
            StartCoroutine(WaitCoroutine());
        else
            StartCoroutine(JumpCoroutine());
    }

    private IEnumerator WaitCoroutine()
    {
        if (!enabled) yield break;
        yield return _waitStartingTime;
        StartCoroutine(JumpCoroutine());
    }

    private IEnumerator JumpCoroutine()
    {
        if (!enabled) yield break;
        hasSplashed = 1;
        yield return _waitUntilJumpStopTimeHalf;
        transform.DOMoveY(_startPos.y + jumpHeight, jumpSpeed).SetEase(Ease.InOutQuad);
        yield return _waitUntilJumpStopTimeHalf;
        my3dModel.DORotate(new Vector3(0, 0, 90f), jumpSpeed).SetEase(Ease.InOutQuad);
        yield return _waitUntilJumpStopTime;
        hasSplashed = -1;
        transform.DOMoveY(_startPos.y, jumpSpeed).SetEase(Ease.InOutQuad);
        //my3dModel.DORotate(new Vector3(0, 0, 0), jumpSpeed * 1.5f);
        yield return _waitUntilJumpStopTimeHalf;
        my3dModel.DORotate(new Vector3(0, 0, -90f), jumpSpeed * 1.5f).SetEase(Ease.InOutQuad);
        yield return _waitUntilJumpStopTimeHalf;
        yield return _waitUntilJumpStopTime;
        StartCoroutine(JumpCoroutine());
    }

    public void Stop()
    {
        DOTween.Kill(transform);
        DOTween.Kill(my3dModel);
        StopAllCoroutines();
    }

    private void Update()
    {
        if (!enabled) return;
        if (hasSplashed == 1)
            if (transform.position.y >= _startPos.y + waterHeight)
            {
                hasSplashed = 0;
                splashParticles.Emit(20);
                splashInSound.Play(audioSource);
            }

        if (hasSplashed == -1)
            if (transform.position.y <= _startPos.y + waterHeight)
            {
                hasSplashed = 0;
                splashParticles.Emit(20);
                splashOutSound.Play(audioSource);
            }
    }


    //ON DRAW GIZMOS
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, jumpHeight, 0), 0.5f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, waterHeight, 0), 0.5f);
    }
}