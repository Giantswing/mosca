using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FlipSystem))]
public class CameraZoneReader : MonoBehaviour
{
    private bool _isInZone = false;
    private CameraZone _cameraZone;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CameraZone cameraZone))
        {
            _cameraZone = cameraZone;
            _isInZone = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_isInZone)
        {
            TargetGroupControllerSystem.SetCameraZoneOffset(_cameraZone.cameraOffset, _cameraZone.cameraZoom,
                _cameraZone.sideAngleStrength);

            if (_cameraZone.isCameraTarget)
                TargetGroupControllerSystem.ModifyTarget(_cameraZone.cameraTarget.transform, 3, 0, 2);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CameraZone cameraZone))
        {
            _isInZone = false;

            TargetGroupControllerSystem.SetCameraZoneOffset(Vector3.zero, 0, 1);

            if (cameraZone.isCameraTarget)
                TargetGroupControllerSystem.ModifyTarget(cameraZone.cameraTarget.transform, 0, 0, 0.5f);
        }
    }
}