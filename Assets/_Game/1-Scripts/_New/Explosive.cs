using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Explosive : MonoBehaviour
{
    public float magnitudeThreshold = 0.5f;
    public UnityEvent OnExplode;
    private Rigidbody rb;
    private GameObject explosionPrefab;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        explosionPrefab = Resources.Load<GameObject>("prefabs/Explosion");
    }

    private void Start()
    {
    }

    public void EnableExplosive()
    {
        enabled = true;
    }

    public void DisableExplosive()
    {
        enabled = false;
    }

    public void Explode()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity).GetComponent<ExplosionScript>()
            .rbToIgnore = rb;
        ScreenFXSystem.ShakeCamera(.7f, 4f);
        ScreenFXSystem.FreezeFrames(.3f);
        OnExplode?.Invoke();
        DisableExplosive();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!enabled) return;
        if (rb.velocity.magnitude > magnitudeThreshold) Explode();
    }
}