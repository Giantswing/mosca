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

        var dir = (transform.position - _stats.dmgDirection).normalized;

        DOTween.Kill(transform);

        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.velocity = dir * 14f;

        transform.rotation = quaternion.identity;

        transform.DORotate(new Vector3(0, 0, 360), 0.35f, RotateMode.FastBeyond360)
            .SetLoops(10, LoopType.Incremental)
            .SetEase(Ease.Linear);

        yield return new WaitForSeconds(0.6f);

        FXMaster.SpawnFX(transform.position, (int)FXListAuto.SmokePuff);

        Destroy(gameObject);
    }

    private void OnDisable()
    {
        DOTween.Kill(transform);
    }
}