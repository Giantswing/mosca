using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class InsecticideCan : MonoBehaviour
{
    [SerializeField] private float timeBetweenBursts, burstDuration;
    [SerializeField] private float particleTimer;
    [SerializeField] private Collider damageCollider;


    private float _currentBurstTimeBetweenBurst, _currentBurstDuration, _currentParticleTimer;

    [SerializeField] private bool isBursting = false;

    [SerializeField] private ParticleSystem particles;

    private WaitForSeconds _waitUntilDamageCollider;

    private void Start()
    {
        _currentBurstDuration = 0;
        _currentBurstTimeBetweenBurst = 0;
        isBursting = false;
        _waitUntilDamageCollider = new WaitForSeconds(0.5f);
        damageCollider.enabled = false;
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
        }

        if (_currentBurstTimeBetweenBurst >= timeBetweenBursts)
        {
            isBursting = true;
            _currentBurstTimeBetweenBurst = 0;
            StartCoroutine(StartDamageColliderRoutine());
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