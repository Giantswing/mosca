using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class TargetGroupControllerSystem : MonoBehaviour
{
    private CinemachineTargetGroup _targetGroup;
    private static TargetGroupControllerSystem Instance;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Ease _easeOut;

    [FormerlySerializedAs("cameraData")] [SerializeField]
    private List<AttributeDataSO> playerList = new();

    private CinemachineTransposer transposer;
    private CinemachineComposer composer;
    private Vector3 startingPositionOffset;
    private Vector3 startingAimOffset;

    private Vector3 positionOffset;
    private Vector3 aimOffset;

    private Vector3 positionOffsetTo;
    private Vector3 aimOffsetTo;

    [SerializeField] private float posOffsetSpeed = 2f;
    [SerializeField] private float aimOffsetSpeed = 2f;
    [SerializeField] private float flipStrength = 2f;

    private void Awake()
    {
        _targetGroup = GetComponent<CinemachineTargetGroup>();
        //_targets.Add(_targetGroup.m_Targets[0].target);
        Instance = this;

        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
        startingPositionOffset = transposer.m_FollowOffset;
        startingAimOffset = composer.m_TrackedObjectOffset;
    }

    public static void ModifyTargetImmediate(Transform target, float weight, float radius)
    {
        int index = Instance._targetGroup.FindMember(target);
        if (index == -1)
            return;

        Instance._targetGroup.m_Targets[index].weight = weight;
        Instance._targetGroup.m_Targets[index].radius = radius;
    }

    public static void ModifyTarget(Transform target, float weight, float radius, float duration = 2f)
    {
        int index = Instance._targetGroup.FindMember(target);
        if (index == -1)
            return;

        float weightTo = weight;
        float radiusTo = radius;
        DOTween.To(() => Instance._targetGroup.m_Targets[index].weight,
            x => Instance._targetGroup.m_Targets[index].weight = x, weightTo,
            duration).SetEase(Ease.InOutQuad);
        DOTween.To(() => Instance._targetGroup.m_Targets[index].radius,
            x => Instance._targetGroup.m_Targets[index].radius = x, radiusTo,
            duration).SetEase(Ease.InOutQuad);
    }

    public static void AddTarget(Transform target, float weight, float radius, float duration = 2f,
        bool useDuration = true)
    {
        for (var i = 0; i < Instance._targetGroup.m_Targets.Length; i++)
            if (Instance._targetGroup.m_Targets[i].target == target)
                return;


        float weightTo = weight;


        if (useDuration)
        {
            Instance._targetGroup.AddMember(target, 0, radius);

            DOTween.To(() => Instance._targetGroup.m_Targets[Instance._targetGroup.m_Targets.Length - 1].weight,
                x => Instance._targetGroup.m_Targets[Instance._targetGroup.m_Targets.Length - 1].weight = x, weightTo,
                duration).SetEase(Ease.InOutQuad);
        }
        else
        {
            Instance._targetGroup.AddMember(target, weight, radius);
        }

        if (target.TryGetComponent(out Attributes attributes))
            if (attributes.hasInstantiatedData)
                Instance.playerList.Add(attributes.attributeData);
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

    public static Transform ClosestPlayer(Transform from)
    {
        if (Instance.playerList.Count == 0)
            return null;

        Transform closest = Instance.playerList[0].attributes.transform;
        float closestDistance = Vector3.Distance(from.position, closest.position);

        for (var i = 1; i < Instance.playerList.Count; i++)
        {
            float distance = Vector3.Distance(from.position, Instance.playerList[i].attributes.transform.position);
            if (distance < closestDistance)
            {
                closest = Instance.playerList[i].attributes.transform;
                closestDistance = distance;
            }
        }

        return closest;
    }

    private void FixedUpdate()
    {
        positionOffsetTo = Vector3.zero;
        aimOffsetTo = Vector3.zero;
        float finalFlipStrength = 0;

        if (playerList.Count > 0)
            finalFlipStrength = flipStrength / playerList.Count;

        for (var i = 0; i < playerList.Count; i++)
        {
            /*
            positionOffsetTo.x -= cameraData[i].movementSystem.direction.x;
            positionOffsetTo.y -= cameraData[i].movementSystem.direction.y;

            aimOffsetTo.x += cameraData[i].movementSystem.direction.x;
            aimOffsetTo.y += cameraData[i].movementSystem.direction.y;
            */

            positionOffsetTo.x += finalFlipStrength * playerList[i].flipSystem.flipDirection;
            aimOffsetTo.x += finalFlipStrength * playerList[i].flipSystem.flipDirection;
        }

        positionOffset = Vector3.Lerp(positionOffset, positionOffsetTo, Time.deltaTime * posOffsetSpeed);
        aimOffset = Vector3.Lerp(aimOffset, aimOffsetTo, Time.deltaTime * aimOffsetSpeed);

        transposer.m_FollowOffset = startingPositionOffset + positionOffset;
        composer.m_TrackedObjectOffset = startingAimOffset + aimOffset;
    }
}