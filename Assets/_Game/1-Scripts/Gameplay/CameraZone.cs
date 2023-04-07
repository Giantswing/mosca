using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZone : MonoBehaviour
{
    public Vector3 cameraOffset;

    [Range(-3, 3)] public float cameraZoom;

    [Range(-2, 2)] public float sideAngleStrength = 1;

    [SerializeField] private bool disableFlip = false;

    public bool isCameraTarget;
    public bool playerInside;
    private float weight;
    private float weightTo;

    private void Start()
    {
        if (isCameraTarget)
            TargetGroupControllerSystem.AddTarget(transform, 0, 0);
    }

    private void Update()
    {
        if (!isCameraTarget) return;
        weight = Mathf.Lerp(weight, weightTo, Time.deltaTime * 2);
        if (weight < 0.05)
            weight = 0;

        weightTo = playerInside ? 3 : 0;
        TargetGroupControllerSystem.ModifyTargetImmediate(transform, weight, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(GetComponent<BoxCollider>().bounds.center, GetComponent<BoxCollider>().bounds.size);
    }
}