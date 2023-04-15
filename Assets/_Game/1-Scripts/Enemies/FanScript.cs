using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;


public class FanScript : MonoBehaviour
{
    [SerializeField] private Transform my3dModel;
    [SerializeField] private float rotationSpeed = 1f;

    /*

    [SerializeField] private WindFxScript windParticlePrefab;
    private ObjectPool<WindFxScript> _windParticlePool;

    private WaitForSeconds _spawnDelay;

    [SerializeField] private float spawnDelayTime;
    [SerializeField] private Transform spawnPos;
    */

    [SerializeField] private int _rayCount = 2;
    [SerializeField] private float _rayOffset = 0.5f;

    private Ray[] _rays;
    private RaycastHit[] _hits;
    private Vector3[] _rayOrigins;

    private Transform _myTransform;
    private readonly float _updateRayTimer = 0.001f;
    private float _timer;
    private float _strength;
    [SerializeField] private float rayMaxLength = 10f;

    private PlayerMovement _playerMovement;
    public LayerMask IgnoreLayer;


    private void Start()
    {
        _myTransform = transform;

        _rays = new Ray[_rayCount];
        _hits = new RaycastHit[_rayCount];
        _rayOrigins = new Vector3[_rayCount];

        for (var i = 0; i < _rayCount; i++)
        {
            Vector3 up = _myTransform.up;
            _rayOrigins[i] = _myTransform.position + up * _rayCount * _rayOffset - up * i * _rayOffset;

            _rays[i] = new Ray(_rayOrigins[i], _myTransform.right);
        }
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > _updateRayTimer)
        {
            _timer = 0;

            for (var i = 0; i < _rays.Length; i++)
            {
                Vector3 up = _myTransform.up;
                _rayOrigins[i] = _myTransform.position + up * ((_rayCount - 1) * _rayOffset * 0.5f) -
                                 up * (i * _rayOffset);

                _rays[i].origin = _rayOrigins[i];
                _rays[i].direction = _myTransform.right;


                if (Physics.Raycast(_rays[i], out _hits[i], rayMaxLength, ~IgnoreLayer))
                    if (_hits[i].collider.TryGetComponent(out Rigidbody rb))
                        rb.AddForceAtPosition(transform.right * 50f, transform.position, ForceMode.Acceleration);


                Debug.DrawLine(_rayOrigins[i], _hits[i].point, Color.red);
            }
        }
    }


    private void OnEnable()
    {
        my3dModel.DOLocalRotate(new Vector3(360, 0, 0), rotationSpeed, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);
    }

    private void OnDisable()
    {
        my3dModel.DOKill();
    }
}