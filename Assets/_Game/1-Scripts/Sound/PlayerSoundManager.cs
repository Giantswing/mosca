using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerSoundManager : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float magnitudeDivider = 10f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    [Space(15)] [SerializeField] private AudioSource flyIdleSound;

    private void Update()
    {
        flyIdleSound.pitch = 1f + rb.velocity.magnitude / magnitudeDivider;
    }
}