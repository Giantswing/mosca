using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class InsecticideCan : MonoBehaviour
{
    [SerializeField] private SimpleAudioEvent gasLeak;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private float timeBetweenBursts, burstDuration;
    [SerializeField] private float particleTimer;
    [SerializeField] private Collider damageCollider;


    private float _currentBurstTimeBetweenBurst, _currentBurstDuration, _currentParticleTimer;

    [SerializeField] private bool isBursting = false;

    [SerializeField] private ParticleSystem particles;

    private WaitForSeconds _waitUntilDamageCollider;

    [SerializeField] private float timeOffset = 0;
    private WaitForSeconds _waitUntilTimeOffset;
    private bool started = false;

    private void Start()
    {
        _currentBurstDuration = 0;
        _currentBurstTimeBetweenBurst = 0;
        isBursting = false;
        _waitUntilDamageCollider = new WaitForSeconds(0.5f);
        damageCollider.enabled = false;
        started = true;

        if (timeOffset > 0)
        {
            started = false;
            _waitUntilTimeOffset = new WaitForSeconds(timeOffset);
            StartCoroutine(StartDelayCoroutine());
        }
    }

    private IEnumerator StartDelayCoroutine()
    {
        yield return _waitUntilTimeOffset;
        started = true;
    }

    private IEnumerator StartDamageColliderRoutine()
    {
        yield return _waitUntilDamageCollider;
        damageCollider.enabled = true;
    }

    private void OnDisable()
    {
        DOTween.Kill(transform);
    }

    private void Update()
    {
        if (!started) return;

        if (isBursting)
            _currentBurstDuration += Time.deltaTime;
        else
            _currentBurstTimeBetweenBurst += Time.deltaTime;

        if (_currentBurstDuration >= burstDuration)
        {
            isBursting = false;
            damageCollider.enabled = false;
            _currentBurstDuration = 0;
            _currentBurstTimeBetweenBurst = 0;
            audioSource.Stop();
        }

        if (_currentBurstTimeBetweenBurst >= timeBetweenBursts)
        {
            isBursting = true;
            _currentBurstTimeBetweenBurst = 0;
            StartCoroutine(StartDamageColliderRoutine());
            gasLeak.Play(audioSource, transform.position);
            transform.DOShakePosition(burstDuration, 0.1f, 10, 90, false, true);
        }

        if (isBursting)
        {
            _currentParticleTimer += Time.deltaTime;

            if (_currentParticleTimer >= particleTimer)
            {
                _currentParticleTimer = 0;
                particles.Emit(1);
            }
        }
    }
}