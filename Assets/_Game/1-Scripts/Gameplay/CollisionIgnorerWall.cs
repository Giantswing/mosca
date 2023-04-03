using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CollisionIgnorerWall : MonoBehaviour
{
    private CollisionIgnorer collisionIgnorer;
    private MeshRenderer meshRenderer;
    private static readonly int ArcOffset = Shader.PropertyToID("_ArcOffset");

    private void Awake()
    {
        collisionIgnorer = GetComponent<CollisionIgnorer>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        collisionIgnorer.onCollisionDetected += OnCollisionDetected;
    }

    private void OnDisable()
    {
        collisionIgnorer.onCollisionDetected -= OnCollisionDetected;
    }

    private void OnCollisionDetected(Vector3 pos)
    {
        /*
        DOTween.To(meshRenderer.material.SetFloat("_ArcOffset",x), x => meshRenderer.material.SetFloat("_ArcOffset", x), 1, 0.5f);
        */

        float value = pos.x > transform.position.x ? -1 : 1;

        meshRenderer.material.SetFloat(ArcOffset, 0);

        DOTween.To(() => meshRenderer.material.GetFloat(ArcOffset), x => meshRenderer.material.SetFloat(ArcOffset, x),
            value, 0.2f).SetLoops(1, LoopType.Yoyo).onComplete += () =>
            DOTween.To(() => meshRenderer.material.GetFloat(ArcOffset),
                x => meshRenderer.material.SetFloat(ArcOffset, x), 0, 0.2f);
    }
}