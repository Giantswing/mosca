using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CannonBullet : MonoBehaviour, IPressurePlateListener, ICollisionIgnore
{
    public Rigidbody rb;
    public bool isAlive = true;
    public HandCannon cannonParent;
    public Collider bulletCollider;
    public Vector3 explosionDirection;

    public void Initialize(float lifeTime)
    {
        isAlive = true;
        transform.localScale = Vector3.one;
        DOVirtual.DelayedCall(lifeTime, OnLifeEnd);
    }


    private void OnLifeEnd()
    {
        if (isAlive)
        {
            FXMaster.SpawnFX(transform.position, (int)FXListAuto.BulletBreak, null, explosionDirection);
            cannonParent.ReturnBulletToStack(this);
            isAlive = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent(out IGenericInteractable interactable))
            interactable.Interact(transform.position);

        OnLifeEnd();
    }

    public Collider GetCollider()
    {
        return bulletCollider;
    }
}