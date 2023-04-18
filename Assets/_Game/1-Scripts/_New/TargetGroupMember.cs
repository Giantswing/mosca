using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class TargetGroupMember : MonoBehaviour
{
    [SerializeField] private float startingWeight = 1f;
    [SerializeField] private float startingRadius = 1f;

    private void OnEnable()
    {
        DOVirtual.DelayedCall(0.1f,
            () =>
            {
                TargetGroupControllerSystem.AddTarget(transform, startingWeight, startingRadius, 0, false);

                TargetGroupControllerSystem.ModifyTarget(transform, startingWeight, startingRadius);
            });
    }

    private void OnDestroy()
    {
        TargetGroupControllerSystem.RemoveTarget(transform);
    }

    /*
    private void OnDisable()
    {
        TargetGroupControllerSystem.RemoveTarget(transform);
    }*/
}