using System;
using DG.Tweening;
using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeed;
    [SerializeField] private Ease ease;


    private void OnEnable()
    {
        transform.DORotate(rotationSpeed, 1f, RotateMode.FastBeyond360).SetEase(ease)
            .SetLoops(-1, LoopType.Incremental);
    }

    private void OnDisable()
    {
        transform.DOKill();
    }
}