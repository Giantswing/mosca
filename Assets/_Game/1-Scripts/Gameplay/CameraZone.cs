using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZone : MonoBehaviour
{
    public Vector3 cameraOffset;

    [Range(-3, 3)] public float cameraZoom;

    [Range(-2, 2)] public float sideAngleStrength = 1;
}