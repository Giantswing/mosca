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
    private readonly float _updateRayTimer = 0.025f;
    private float _timer;
    private float _strength;
    [SerializeField] private float rayMaxLength = 10f;

    private PlayerMovement _playerMovement;
    public LayerMask IgnoreLayer;


    private void Start()
    {
        /*
        _spawnDelay = new WaitForSeconds(spawnDelayTime);

        _windParticlePool = new ObjectPool<WindFxScript>(
            () => Instantiate(windParticlePrefab),
            windObj =>
            {
                windObj.gameObject.SetActive(true);
                windObj.moveDir = transform.right;
            },
            windObj => { windObj.gameObject.SetActive(false); },
            windObj => { Destroy(windObj); },
            false, 3, 3);

        StartCoroutine(BlowWindCoroutine());
        */

        _myTransform = transform;

        _rays = new Ray[_rayCount];
        _hits = new RaycastHit[_rayCount];
        _rayOrigins = new Vector3[_rayCount];

        for (var i = 0; i < _rayCount; i++)
        {
            var up = _myTransform.up;
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
                var up = _myTransform.up;
                _rayOrigins[i] = _myTransform.position + up * ((_rayCount - 1) * _rayOffset * 0.5f) -
                                 up * (i * _rayOffset);

                _rays[i].origin = _rayOrigins[i];
                _rays[i].direction = _myTransform.right;

                //raycast ignore triggers


                if (Physics.Raycast(_rays[i], out _hits[i], rayMaxLength, ~IgnoreLayer))
                    //check the tag to see if it's a player
                    if (_hits[i].collider.CompareTag("Player"))
                    {
                        if (_playerMovement == null)
                            _playerMovement = _hits[i].collider.GetComponent<PlayerMovement>();

                        _strength = 1 - _hits[i].distance / rayMaxLength;
                        _playerMovement.windForceTo += transform.right * (_strength * 4f);
                        _playerMovement.currentlyInWind = 0;
                    }

                Debug.DrawLine(_rayOrigins[i], _hits[i].point, Color.red);
            }
        }
    }

    /*
    private IEnumerator BlowWindCoroutine()
    {
        var windObj = _windParticlePool.Get();
        windObj.Init(EndObj);
        windObj.transform.position = spawnPos.transform.position + transform.up * Random.Range(-0.5f, 0.5f);
        windObj.transform.rotation = transform.rotation;
        yield return _spawnDelay;

        StartCoroutine(BlowWindCoroutine());
    }

    private void EndObj(WindFxScript wind)
    {
        _windParticlePool.Release(wind);
    }
    */

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