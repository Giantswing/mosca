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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(GetComponent<BoxCollider>().bounds.center, GetComponent<BoxCollider>().bounds.size);
    }
}