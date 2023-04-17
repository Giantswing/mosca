using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Attributes))]
[RequireComponent(typeof(DashAbility))]
[RequireComponent(typeof(LookAtRotation))]
public class DoubleDashAbility : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 direction;
    private Attributes attributes;
    private DashAbility dashAbility;
    private LookAtRotation lookAtRotation;
    public float speedBoost = 2f;

    [SerializeField] private float delayToStartThreshold = .5f;
    [SerializeField] private float thresholdDuration = .15f;
    public float dashCooldown = 1f;


    public UnityEvent<string, bool> OnDoubleDash;
    public UnityEvent OnDoubleDashGeneric;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        attributes = GetComponent<Attributes>();
        dashAbility = GetComponent<DashAbility>();
        lookAtRotation = GetComponent<LookAtRotation>();
    }

    public void EnableDoubleDash(Vector3 direction)
    {
        DOVirtual.DelayedCall(delayToStartThreshold, () =>
        {
            enabled = true;
            DOVirtual.DelayedCall(thresholdDuration, () => { enabled = false; });
        });
    }

    public void DoubleDash(Vector3 direction)
    {
        if (!enabled || rb.velocity.magnitude < 4f) return;

        enabled = false;
        dashAbility.enabled = false;
        dashAbility.DisableDashRecovery();
        lookAtRotation.useAngleRotation = true;
        attributes.damagePriority = 1;

        this.direction = direction;

        rb.AddForce(direction * attributes.acceleration * speedBoost, ForceMode.Acceleration);
        OnDoubleDash?.Invoke("IsDoubleDashing", true);
        OnDoubleDashGeneric?.Invoke();
        attributes.canDoDamage = true;
        attributes.canInteract = true;

        SoundMaster.PlaySound(transform.position, (int)SoundListAuto.FlyDodge);

        DOVirtual.DelayedCall(dashCooldown * .5f, () =>
        {
            attributes.damagePriority = -1;
            attributes.canDoDamage = false;
            attributes.canInteract = false;
            lookAtRotation.useAngleRotation = false;
            OnDoubleDash?.Invoke("IsDoubleDashing", false);
        });

        DOVirtual.DelayedCall(dashCooldown, () => { dashAbility.enabled = true; });
    }


    private void Start()
    {
    }
}