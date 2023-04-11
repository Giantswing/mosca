using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CollisionIgnorerWall : MonoBehaviour
{
    [SerializeField] private bool isActivated = true;
    [Space(25)] private CollisionIgnorer collisionIgnorer;
    private MeshRenderer meshRenderer;
    private static readonly int ArcOffset = Shader.PropertyToID("_ArcOffset");
    private static readonly int Opacity = Shader.PropertyToID("_Opacity");

    [SerializeField] private BoxCollider collisionCollider;
    [SerializeField] private BoxCollider triggerCollider;


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

    public void Deactivate()
    {
        if (!isActivated) return;

        DOTween.To(() => meshRenderer.material.GetFloat(Opacity), x => meshRenderer.material.SetFloat(Opacity, x),
            0, 0.2f).onComplete += () =>
        {
            triggerCollider.enabled = false;
            collisionCollider.enabled = false;
            isActivated = false;
        };
    }

    public void Activate()
    {
        if (isActivated) return;

        DOTween.To(() => meshRenderer.material.GetFloat(Opacity), x => meshRenderer.material.SetFloat(ArcOffset, x),
            1, 0.2f).onComplete += () =>
        {
            triggerCollider.enabled = true;
            collisionCollider.enabled = true;
            isActivated = true;
        };
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