using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushable : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        audioSource.volume = rb.velocity.magnitude / 7f;
    }
}