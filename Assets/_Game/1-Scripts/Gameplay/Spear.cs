using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class Spear : MonoBehaviour
{
    public Rigidbody myRb;
    public BoxCollider myCollider;
    [HideInInspector] public CapsuleCollider parentCollider;
    public ScarabWarrior myScarabWarrior;
    [HideInInspector] public BoxCollider shieldCollider;
    [HideInInspector] public float zDepthTo;
    private float zDepth;

    public enum upVectorTypes
    {
        Vector3Up,
        Vector3Forward,
        Vector3Right
    }

    [SerializeField] private upVectorTypes UpVector;

    private Vector3 upVector;
    public bool hasCollided = false;

    public void Initialize()
    {
        DOVirtual.DelayedCall(1.5f,
            () => { transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).onComplete += () => { Return(); }; });

        myCollider.enabled = false;

        DOVirtual.DelayedCall(0.05f, () => { myCollider.enabled = true; });


        switch (UpVector)
        {
            case upVectorTypes.Vector3Up:
                upVector = Vector3.up;
                break;

            case upVectorTypes.Vector3Forward:
                upVector = Vector3.forward;
                break;

            case upVectorTypes.Vector3Right:
                upVector = Vector3.right;
                break;
        }
    }

    private void Return()
    {
        myScarabWarrior.ReturnSpear(this);
        hasCollided = false;
    }

    private void Update()
    {
        zDepth = Mathf.Lerp(zDepth, zDepthTo, 0.1f);

        if (!hasCollided)
            transform.rotation = Quaternion.LookRotation(myRb.velocity, upVector);

        transform.position = new Vector3(transform.position.x, transform.position.y, zDepth);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasCollided)
        {
            myCollider.enabled = false;
            transform.parent = collision.transform;
            transform.DOPunchRotation(Vector3.up * 40, 1f, 10, 1f);
            hasCollided = true;
            myRb.isKinematic = true;
            myRb.velocity = Vector3.zero;
            myRb.angularVelocity = Vector3.zero;
        }
    }
}