using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MinimapUpdater : MonoBehaviour
{
    public int fps = 2;
    private float elapsed;
    private Camera cam;
    [SerializeField] private RenderTexture rt;

    private void Start()
    {
        cam = GetComponent<Camera>();
        cam.enabled = false;
    }

    private void Update()
    {
        cam.targetTexture = null;
        elapsed += Time.deltaTime;
        if (elapsed > .2f)
        {
            elapsed = 0;
            cam.targetTexture = rt;
            cam.Render();
        }
    }
}