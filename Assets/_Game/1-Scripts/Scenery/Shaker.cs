using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Shaker : MonoBehaviour
{
    [SerializeField] private Light pointLight;
    [SerializeField] private float moveRange;
    [SerializeField] private float flickerSpeed;
    private float _startingLightIntensity;

    private WaitForSeconds _waitTime;
    private Vector3 _startPos;

    private void Start()
    {
        _startPos = transform.position;
        _waitTime = new WaitForSeconds(flickerSpeed);
        _startingLightIntensity = pointLight.intensity;


        StartCoroutine(Flicker());
    }

    private IEnumerator Flicker()
    {
        yield return _waitTime;
        pointLight.DOIntensity(_startingLightIntensity * Random.Range(0.9f, 1.1f), flickerSpeed)
            .SetEase(Ease.InOutQuad);


        transform.DOMove(
            _startPos + new Vector3(Random.Range(-moveRange, moveRange), Random.Range(-moveRange, moveRange),
                Random.Range(-moveRange, moveRange)), flickerSpeed).SetEase(Ease.InOutQuad);

        StartCoroutine(Flicker());
    }
}