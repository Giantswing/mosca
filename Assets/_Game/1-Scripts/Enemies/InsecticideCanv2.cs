using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class InsecticideCanv2 : MonoBehaviour
{
    [SerializeField] private float burstDuration;
    [SerializeField] private float timeBetweenBursts;
    [SerializeField] private float startingOffsetMultiplier;
    [SerializeField] private float burstGrowthDuration = 1f;
    [SerializeField] private float rayMaxLength = 10f;

    [Space(25)] [SerializeField] private SimpleAudioEvent gasLeak;
    [SerializeField] private AudioSource audioSource;

    [Space(25)] [SerializeField] private ParticleSystem particles;

    private bool _isBursting = false;
    private bool _foundTarget = false;

    [SerializeField] private Transform _myTransform;

    /* Ray stuff */
    private float _targetDistance;
    private int _rayCount = 2;
    private float _rayOffset = 0.75f;
    private Ray[] _rays;
    private RaycastHit[] _hits;
    private Vector3[] _rayOrigins;
    private readonly float _updateRayTimer = 0.025f;
    public LayerMask IgnoreLayer;
    private float _timer;
    private float _clampedDistance;
    private float currentRayLength = 0;

    /* ---------------- */

    private WaitForSeconds _WaitBurstDuration;
    private WaitForSeconds _WaitTimeBetweenBursts;
    private WaitForSeconds _WaitStartingOffset;

    [Space(25)] [SerializeField] private PlayerReferenceSO playerReference;


    private void Start()
    {
        _rays = new Ray[_rayCount];
        _hits = new RaycastHit[_rayCount];
        _rayOrigins = new Vector3[_rayCount];

        for (var i = 0; i < _rayCount; i++)
        {
            var up = _myTransform.up;
            _rayOrigins[i] = _myTransform.position + up * _rayCount * _rayOffset - up * i * _rayOffset;

            _rays[i] = new Ray(_rayOrigins[i], _myTransform.right);
        }

        _WaitBurstDuration = new WaitForSeconds(burstDuration);
        _WaitTimeBetweenBursts = new WaitForSeconds(timeBetweenBursts);
        _WaitStartingOffset = new WaitForSeconds((burstDuration + timeBetweenBursts) * startingOffsetMultiplier);

        particles.Stop();

        if (startingOffsetMultiplier > 0)
            StartCoroutine(StartingOffsetCoroutine());
        else
            StartCoroutine(BurstCoroutine());
    }

    /* Coroutines */

    private IEnumerator StartingOffsetCoroutine()
    {
        yield return _WaitStartingOffset;
        StartCoroutine(BurstCoroutine());
    }

    private IEnumerator BurstCoroutine()
    {
        yield return _WaitTimeBetweenBursts;
        particles.Play();
        gasLeak.Play(audioSource, transform.position);
        _targetDistance = rayMaxLength;

        transform.DOKill();
        transform.DOShakePosition(burstDuration * 2f, 0.1f, 10, 90, false, true);
        transform.DOShakeScale(burstDuration * 2f, 0.03f, 10, 90, false);

        _isBursting = true;
        currentRayLength = 0;
        DOTween.To(() => currentRayLength, x => currentRayLength = x, rayMaxLength, burstGrowthDuration);
        yield return _WaitBurstDuration;
        _isBursting = false;
        particles.Stop();

        StartCoroutine(BurstCoroutine());
    }

    /* ---------------- */

    /*
    private Vector3 getStartingPos()
    {
        return _myTransform.position + _myTransform.right * (0.2f + Mathf.Min(currentRayLength, _targetDistance) * .5f);
    }

    private Vector3 getColliderSize()
    {
        return new Vector3(0.5f, currentRayLength, 0.5f);
    }
    */

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < _updateRayTimer || !_isBursting) return;

        _timer = 0;


        for (var i = 0; i < _rays.Length; i++)
        {
            var foundTarget = false;
            var up = _myTransform.up;
            _rayOrigins[i] = _myTransform.position + up * ((_rayCount - 1) * _rayOffset * 0.5f) -
                             up * (i * _rayOffset);

            _rays[i].origin = _rayOrigins[i];
            _rays[i].direction = _myTransform.right;
            _clampedDistance = Mathf.Min(currentRayLength, _targetDistance + 0.1f);

            if (Physics.Raycast(_rays[i], out _hits[i], _clampedDistance, ~IgnoreLayer))
            {
                foundTarget = true;
                _targetDistance = _hits[i].distance;

                if (_hits[i].collider.CompareTag("Player"))
                {
                    _isBursting = false;
                    playerReference.playerInteractionHandler.CheckTakeDamage(1, _hits[i].point);
                }
            }


            if (_isBursting)
            {
                if (foundTarget)
                    Debug.DrawLine(_rayOrigins[i],
                        _rayOrigins[i] + _myTransform.right * _clampedDistance, Color.red);
                else
                    Debug.DrawLine(_rayOrigins[i],
                        _rayOrigins[i] + _myTransform.right * _clampedDistance,
                        Color.green);
            }
        }
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(getStartingPos(),
            getColliderSize());
    }
    */
}