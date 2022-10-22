using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZone : MonoBehaviour
{
    public Vector3 cameraOffset;

    [Range(-1, 1)] public float cameraZoom;
}