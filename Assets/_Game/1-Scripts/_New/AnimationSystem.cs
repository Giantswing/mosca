using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSystem : MonoBehaviour
{
    public Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
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

    private void Start()
    {
    }
}