using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Attributes))]
[RequireComponent(typeof(LookAtRotation))]
public class DashAbility : MonoBehaviour
{
    private Rigidbody rb;
    private Attributes attributes;
    public float dashCooldown = 1f;
    public float speedBoost = 1.5f;
    public UnityEvent<string, bool> OnDash;
    public UnityEvent<Vector3> OnDashDirection;
    private LookAtRotation lookAtRotation;

    private Tween delayedActivation;
    private Tween delayedRestore;

    private bool canSpeedBoost = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        attributes = GetComponent<Attributes>();
        lookAtRotation = GetComponent<LookAtRotation>();
    }

    public void RemoveSpeedBoost()
    {
        canSpeedBoost = false;
    }

    public void RestoreSpeedBoost()
    {
        DOVirtual.DelayedCall(0.1f, () => { canSpeedBoost = true; });
    }


    public void Dash(Vector3 direction)
    {
        if (!enabled) return;

        enabled = false;

        if (canSpeedBoost)
            rb.AddForce(direction * attributes.acceleration * speedBoost, ForceMode.Acceleration);
        else
            //brake the player a lot, but not completely stop him
            rb.AddForce(-direction * attributes.acceleration * speedBoost * .5f, ForceMode.Acceleration);

        OnDash?.Invoke("IsDashing", true);
        OnDashDirection?.Invoke(direction);
        attributes.canDoDamage = true;
        attributes.canInteract = true;
        lookAtRotation.useAngleRotation = true;
        SoundMaster.PlaySound(transform.position, (int)SoundListAuto.FlyDodge);

        delayedRestore = DOVirtual.DelayedCall(dashCooldown * .5f, () =>
        {
            attributes.canDoDamage = false;
            attributes.canInteract = false;
            OnDash?.Invoke("IsDashing", false);
            lookAtRotation.useAngleRotation = false;
        });

        delayedActivation = DOVirtual.DelayedCall(dashCooldown, () => { enabled = true; });
    }

    public void DisableDashRecovery()
    {
        OnDash?.Invoke("IsDashing", false);
        delayedActivation.Kill();
        delayedRestore.Kill();
    }

    private void Start()
    {
    }
}