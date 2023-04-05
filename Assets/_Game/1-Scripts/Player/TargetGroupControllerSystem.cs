using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class TargetGroupControllerSystem : MonoBehaviour
{
    private CinemachineTargetGroup _targetGroup;
    private static TargetGroupControllerSystem Instance;
    [SerializeField] private List<Transform> _targets = new();
    [SerializeField] private Ease _easeOut;

    private void Awake()
    {
        _targetGroup = GetComponent<CinemachineTargetGroup>();
        _targets.Add(_targetGroup.m_Targets[0].target);
        Instance = this;
    }

    public static void ModifyTargetImmediate(Transform target, float weight, float radius)
    {
        var index = Instance._targetGroup.FindMember(target);
        if (index == -1)
            return;

        Instance._targetGroup.m_Targets[index].weight = weight;
        Instance._targetGroup.m_Targets[index].radius = radius;
    }

    public static void ModifyTarget(Transform target, float weight, float radius, float duration = 2f)
    {
        var index = Instance._targetGroup.FindMember(target);
        if (index == -1)
            return;

        var weightTo = weight;
        var radiusTo = radius;
        DOTween.To(() => Instance._targetGroup.m_Targets[index].weight,
            x => Instance._targetGroup.m_Targets[index].weight = x, weightTo,
            duration).SetEase(Ease.InOutQuad);
        DOTween.To(() => Instance._targetGroup.m_Targets[index].radius,
            x => Instance._targetGroup.m_Targets[index].radius = x, radiusTo,
            duration).SetEase(Ease.InOutQuad);
    }

    public static void AddTarget(Transform target, float weight, float radius, float duration = 2f)
    {
        for (var i = 0; i < Instance._targetGroup.m_Targets.Length; i++)
            if (Instance._targetGroup.m_Targets[i].target == target)
                return;


        var weightTo = weight;
        Instance._targetGroup.AddMember(target, 0, radius);


        DOTween.To(() => Instance._targetGroup.m_Targets[Instance._targetGroup.m_Targets.Length - 1].weight,
            x => Instance._targetGroup.m_Targets[Instance._targetGroup.m_Targets.Length - 1].weight = x, weightTo,
            duration).SetEase(Ease.InOutQuad);
    }


    public static void RemoveTarget(Transform target)
    {
        if (Instance._targetGroup.FindMember(target) == -1)
            return;

        DOTween.To(() => Instance._targetGroup.m_Targets[Instance._targetGroup.m_Targets.Length - 1].weight,
            x => Instance._targetGroup.m_Targets[Instance._targetGroup.m_Targets.Length - 1].weight = x, 0,
            1f).SetEase(Instance._easeOut).OnComplete(() =>
        {
            Instance._targetGroup.RemoveMember(target);
            Instance._targetGroup.m_Targets[0].weight = 1;
        });
    }
}