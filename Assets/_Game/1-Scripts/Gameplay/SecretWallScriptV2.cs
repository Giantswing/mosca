using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SecretWallScriptV2 : MonoBehaviour
{
    [SerializeField] private MeshRenderer occluderObj;
    [SerializeField] private SecretWallScriptV2 otherWall;

    [SerializeField] private MeshRenderer meshRenderer;

    private bool _isDisappearing = false;
    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");

    private void Start()
    {
        occluderObj.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_isDisappearing) return;
            Disappear();
            _isDisappearing = true;
        }
    }

    public void Disappear(bool isCalledFromOtherWall = false)
    {
        Destroy(occluderObj.gameObject);

        if (!isCalledFromOtherWall)
            if (otherWall != null)
                otherWall.Disappear(true);

        for (var i = 0; i < meshRenderer.materials.Length; i++)
            meshRenderer.materials[i].DOFloat(1, DissolveAmount, 2f).OnComplete(() => { Destroy(gameObject); });
    }
}