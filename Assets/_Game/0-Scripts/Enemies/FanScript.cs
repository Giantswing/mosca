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

    [SerializeField] private WindFxScript windParticlePrefab;
    private ObjectPool<WindFxScript> _windParticlePool;

    private WaitForSeconds _spawnDelay;

    [SerializeField] private float spawnDelayTime;
    [SerializeField] private Transform spawnPos;

    private void Start()
    {
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
    }

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