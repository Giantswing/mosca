using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpikeBallEnemy : MonoBehaviour
{
    [SerializeField] private Renderer _myRenderer;
    private Tween _hitTween;

    private void Start()
    {
    }

    public void GotHit()
    {
        _hitTween = DOVirtual.Float(1f, 0.1f, 1.5f, value => _myRenderer.material.SetFloat("_wobbleSpeed", value))
            .SetEase(Ease.InOutBounce);
        _hitTween.Play();
    }

    private void OnDisable()
    {
        _hitTween.Kill(false);
    }
}