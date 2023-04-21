using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class FlipSystem : MonoBehaviour
{
    [SerializeField] private Transform flipTransform;
    [SerializeField] private float xThreshold;
    public int flipDirection = 1;
    public float flipSpeed = 0.5f;

    public Vector3 inputDirection;

    public bool canFlip = true;
    private Rigidbody rb;

    private void Awake()
    {
        if (flipTransform == null)
        {
            if (transform.childCount > 0)
                flipTransform = transform.GetChild(0);
            else
                flipTransform = transform;
        }

        rb = GetComponent<Rigidbody>();
        flipDirection = 1;
    }

    private void Update()
    {
        if (rb.velocity.x > xThreshold)
            Flip(1);
        else if (rb.velocity.x < -xThreshold)
            Flip(-1);
    }

    public void UpdateInputDirection(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            if (inputDirection.x > 0)
                Flip(1);
            else if (inputDirection.x < 0)
                Flip(-1);

            inputDirection = Vector3.zero;
        }
        else
        {
            if (Mathf.Abs(context.ReadValue<Vector2>().x) > 0.85f)
                inputDirection = context.ReadValue<Vector2>();
        }

        /*
        inputDirection = context.ReadValue<Vector2>();
            if (inputDirection.magnitude < 0.2f)
            {
                if (inputDirection.x > 0)
                    Flip(1);
                else if (inputDirection.x < 0)
                    Flip(-1);
            }
            */
    }


    public void Flip(int directionToFlip)
    {
        if (flipDirection == directionToFlip || !canFlip)
            return;

        flipDirection = directionToFlip;

        flipTransform.DOLocalRotate(new Vector3(0, 90, -45f), flipSpeed * 0.5f).SetEase(Ease.Linear)
                .onComplete +=
            () =>
            {
                flipTransform
                    .DOLocalRotate(new Vector3(0, directionToFlip == 1 ? 0 : 180, 0), flipSpeed * 0.5f)
                    .SetEase(Ease.Linear);
            };


        //flipTransform.DORotate(new Vector3(0, directionToFlip == 1 ? 0 : 180, 0), 0.5f, RotateMode.FastBeyond360);
    }

    public void DisableFlip()
    {
        canFlip = false;
        DOVirtual.DelayedCall(0.5f, () => canFlip = true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        DisableFlip();
    }
}