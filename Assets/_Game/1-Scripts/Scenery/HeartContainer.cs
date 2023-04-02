using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HeartContainer : MonoBehaviour
{
    [SerializeField] private Transform inner_heart;
    [SerializeField] private Transform outer_heart;
    private WaitForSeconds coroutineBeatingHeart = new(1f);

    private void Start()
    {
        StartCoroutine(Coroutine_BeatingHeart());
    }

    private IEnumerator Coroutine_BeatingHeart()
    {
        while (true)
        {
            inner_heart.DOPunchScale(Vector3.one * .1f, .2f).onComplete +=
                () => inner_heart.DOPunchScale(Vector3.one * .1f, .2f);
            outer_heart.DOPunchScale(Vector3.one * .2f, .2f).SetDelay(0.1f).onComplete +=
                () => outer_heart.DOPunchScale(Vector3.one * .1f, .2f);
            transform.DOShakeRotation(0.6f, 10f, 10, 90f, false);


            yield return coroutineBeatingHeart;
        }
    }
}