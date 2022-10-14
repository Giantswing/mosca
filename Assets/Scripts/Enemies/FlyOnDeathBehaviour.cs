using System;
using System.Collections;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class FlyOnDeathBehaviour : MonoBehaviour
{
    public void StartFlying()
    {
        StartCoroutine(StartFlyingRoutine());
    }

    private IEnumerator StartFlyingRoutine()
    {
        var _stats = GetComponent<STATS>();

        if (_stats == null) Destroy(gameObject);

        var dir = (_stats.dmgDirection - transform.position).normalized;

        DOTween.Kill(transform);

        GetComponent<Rigidbody>().velocity = transform.position - dir * 9f;

        transform.rotation = quaternion.identity;

        transform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360)
            .SetLoops(10, LoopType.Incremental)
            .SetEase(Ease.Linear);

        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }

    private void OnDisable()
    {
        DOTween.Kill(transform);
    }
}