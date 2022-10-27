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

    private Ray _ray;
    private RaycastHit _hit;
    private Transform _myTransform;
    private readonly float _updateRayTimer = 0.025f;
    private float _timer;
    private float _strength;
    [SerializeField] private float rayMaxLength = 10f;

    private PlayerMovement _playerMovement;

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
        _ray = new Ray(_myTransform.position, _myTransform.right);
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > _updateRayTimer)
        {
            _timer = 0;

            //update raycast
            _ray.origin = _myTransform.position;
            _ray.direction = _myTransform.right;

            if (Physics.Raycast(_ray, out _hit, rayMaxLength))
                //check the tag to see if it's a player
                if (_hit.collider.CompareTag("Player"))
                {
                    if (_playerMovement == null)
                        _playerMovement = _hit.collider.GetComponent<PlayerMovement>();

                    _strength = 1 - _hit.distance / rayMaxLength;
                    _playerMovement.windForceTo += transform.right * (_strength * 12f);
                }

            Debug.DrawLine(_myTransform.position, _hit.point, Color.red);
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