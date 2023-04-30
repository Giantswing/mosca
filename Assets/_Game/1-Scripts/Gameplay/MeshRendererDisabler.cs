using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRendererDisabler : MonoBehaviour
{
    private MeshRenderer[] meshRenderers;

    private void Awake()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in meshRenderers) meshRenderer.enabled = false;
    }
}