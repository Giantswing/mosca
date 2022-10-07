using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SecretWallScript : MonoBehaviour
{
    [SerializeField] private MeshRenderer secretContainer;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshRenderer wallMeshRenderer;

    [SerializeField] private Material fadeMaterial;

    private bool _isDissapearing = false;
    //private Color _bgColor;

    private void Start()
    {
        secretContainer.enabled = true;
        //_bgColor = Camera.main.backgroundColor;
    }

    public void Disappear()
    {
        if (_isDissapearing) return;

        _isDissapearing = true;
        secretContainer.gameObject.layer = 0;
        secretContainer.material = fadeMaterial;
        //secretContainer.material.color = _bgColor;
        secretContainer.material.DOFade(0, 1f);
        wallMeshRenderer.material.DOFade(0, 1f);
        meshRenderer.material.DOFade(0, 1f).onComplete += () =>
        {
            Destroy(secretContainer.gameObject);
            Destroy(gameObject);
        };
    }
}