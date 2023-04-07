using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Reseteable : MonoBehaviour, IPressurePlateListener
{
    public Vector3 startPos;
    public Quaternion startRot;
    public Vector3 startScale;
    public GameObject ghost;

    private void Awake()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        startScale = transform.localScale;
    }

    private void Start()
    {
        if (ghost != null) Instantiate(ghost, transform.position, transform.rotation);
    }


    public void CustomReset()
    {
        //before reset
        transform.DOShakeRotation(1f, 10, 20, 90, false);
        transform.DOShakePosition(1f, 0.2f, 20, 90, false);
        transform.DOScale(startScale * 1.3f, 1f).SetEase(Ease.Linear);
        //after reset

        DOVirtual.DelayedCall(1f, () =>
        {
            FXMaster.SpawnFX(transform.position, (int)FXTypes.Reset);
            transform.localScale = startScale * 0.3f;
            transform.DOScale(startScale, 0.5f).SetEase(Ease.OutBounce).onComplete +=
                () => { transform.localScale = startScale; };
            transform.position = startPos;
            transform.rotation = startRot;
            FXMaster.SpawnFX(transform.position, (int)FXTypes.Reset);
        });
    }
}