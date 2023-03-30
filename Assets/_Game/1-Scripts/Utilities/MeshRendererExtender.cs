using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRendererExtender : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    [SerializeField] private bool _showBounds = true;
    [SerializeField] private Vector3 _boundsOffset = Vector3.zero;
    [SerializeField] private Vector3 _boundsScale = Vector3.one;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();

        RecalculateBounds();
    }

    private void RecalculateBounds()
    {
        if (_meshRenderer == null)
            _meshRenderer = GetComponent<MeshRenderer>();
        if (_meshRenderer == null)
            return;
        var bounds = _meshRenderer.bounds;
        bounds.center += _boundsOffset;
        bounds.extents = Vector3.Scale(bounds.extents, _boundsScale);
        _meshRenderer.bounds = bounds;
    }

    private void OnValidate()
    {
        //RecalculateBounds(); 
    }

    public void OnDrawGizmosSelected()
    {
        var r = GetComponent<Renderer>();
        if (r == null)
            return;
        var bounds = r.bounds;
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(bounds.center, bounds.extents * 2);
    }
}