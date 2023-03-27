using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAdjustments : MonoBehaviour
{
    private Camera _camera;
    [SerializeField] private float _bufferZone = 0.1f;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Start()
    {
        // get the default layer cull distances
        var defaultDistances = new float[32];
        _camera.layerCullDistances.CopyTo(defaultDistances, 0);

        // set the new layer cull distances with the buffer zone size added
        var newDistances = new float[32];
        for (var i = 0; i < newDistances.Length; i++) newDistances[i] = defaultDistances[i] + _bufferZone;
        _camera.layerCullDistances = newDistances;

        print("frostum changed");
    }
}