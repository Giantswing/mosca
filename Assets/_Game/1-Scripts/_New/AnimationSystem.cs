using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Attributes))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(FlipSystem))]
public class AnimationSystem : MonoBehaviour
{
    public Animator animator;
    public Attributes attributes;
    private Rigidbody rb;
    private FlipSystem flipSystem;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        attributes = GetComponent<Attributes>();
        rb = GetComponent<Rigidbody>();
        flipSystem = GetComponent<FlipSystem>();
    }

    public void SetBool(string name, bool value)
    {
        animator.SetBool(name, value);
    }

    public void SetInt(string name, int value)
    {
        animator.SetInteger(name, value);
    }

    public void SetFloat(string name, float value)
    {
        animator.SetFloat(name, value);
    }

    public void SetMultipleFloats(string value1, string value2, Vector3 vector)
    {
        animator.SetFloat(value1, vector.x);
        animator.SetFloat(value2, vector.y);
    }

    public void MicroAnimationCharge(Vector3 direction)
    {
        attributes.objectModel
                .DOLocalMoveX(attributes.objectModel.localPosition.x - 1.6f * flipSystem.flipDirection, .15f)
                .onComplete +=
            () => { attributes.objectModel.DOLocalMoveX(0, .15f).SetEase(Ease.OutQuad); };
    }

    public void MicroAnimationForward()
    {
        attributes.objectModel
                .DOLocalMoveX(attributes.objectModel.localPosition.x + 1f * flipSystem.flipDirection, .35f)
                .onComplete +=
            () => { attributes.objectModel.DOLocalMoveX(0, .35f).SetEase(Ease.OutQuad); };
    }

    private void Start()
    {
    }
}