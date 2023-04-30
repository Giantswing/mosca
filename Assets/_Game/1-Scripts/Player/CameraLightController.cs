using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLightController : MonoBehaviour
{
    [SerializeField] private Light cameraLight;
    [SerializeField] private float minLightStrength = 380f;
    [SerializeField] private float maxLightStrength = 1450f;
    [SerializeField] private float minCameraDistance = 9f;
    [SerializeField] private float maxCameraDistance = 20f;
    [SerializeField] private float distanceMultiplier = 1f;

    private void Update()
    {
        // Get the current camera distance
        float cameraDist = Mathf.Clamp(-transform.position.z, minCameraDistance, maxCameraDistance);

        // Normalize the camera distance between 0 and 1
        float normalizedCameraDist = Mathf.InverseLerp(minCameraDistance, maxCameraDistance, cameraDist);

        // Calculate the light intensity based on the normalized camera distance
        float lightIntensity = Mathf.Lerp(minLightStrength, maxLightStrength, normalizedCameraDist);

        // Set the light intensity
        cameraLight.intensity = lightIntensity;
    }
}