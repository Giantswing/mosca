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
                TargetGroupControllerSystem.AddTarget(transform, 1f, 1f, 0, false);

                TargetGroupControllerSystem.ModifyTarget(transform, startingWeight, startingRadius);
            });
    }

    private void OnDisable()
    {
        TargetGroupControllerSystem.RemoveTarget(transform);
    }
}