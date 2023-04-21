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
    private FlipSystem flipSystem;

    private Tween delayedActivation;
    private Tween delayedRestore;

    private bool canSpeedBoost = true;

    private Collider[] colliders = new Collider[10];
    [SerializeField] private List<Transform> interactables = new();

    private Vector3 startScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        attributes = GetComponent<Attributes>();
        lookAtRotation = GetComponent<LookAtRotation>();
        flipSystem = GetComponent<FlipSystem>();
        startScale = transform.localScale;
    }

    public void RemoveSpeedBoost()
    {
        canSpeedBoost = false;
    }

    public void RestoreSpeedBoost()
    {
        DOVirtual.DelayedCall(0.1f, () => { canSpeedBoost = true; });
    }

    public Vector3 CalculateDashDirection(Vector3 directionIn)
    {
        Vector3 directionOut = directionIn;

        if (directionIn.magnitude <= .25f)
        {
            int numOverlaps = Physics.OverlapSphereNonAlloc(transform.position, 2f, colliders);
            for (var i = 0; i < numOverlaps; i++)
            {
                IInteractableWithDash interactable = colliders[i].GetComponent<IInteractableWithDash>();

                if (interactable != null) interactables.Add(colliders[i].transform);
            }

            float closestDistance = 1000;
            Vector3 closestPosition = Vector3.zero;

            foreach (Transform interactable in interactables)
            {
                if (interactable == null) continue;
                float distance = Vector2.Distance(transform.position, interactable.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPosition = interactable.position;
                }
            }

            if (closestDistance == 1000)
            {
                return new Vector3(flipSystem.flipDirection, 0, 0) * 1.35f;
            }
            else
            {
                Vector3 diff = closestPosition - transform.position;
                float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

                //fit movementDirection to from -1 to 1 depending on angle
                Vector2 movementDirection =
                    new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);


                return movementDirection;
            }
        }
        else
        {
            return directionIn;
        }
    }

    public void Dash(Vector3 direction)
    {
        if (!enabled) return;

        enabled = false;
        transform.localScale = startScale * .5f;
        attributes.damagePriority = 1;

        transform.DOScale(startScale, 0.3f).SetEase(Ease.OutBack).onComplete += () =>
        {
            transform.localScale = startScale;
        };

        if (canSpeedBoost)
            rb.AddForce(CalculateDashDirection(direction) * attributes.acceleration * speedBoost,
                ForceMode.Acceleration);
        else
            //brake the player a lot, but not completely stop him
            rb.AddForce(-CalculateDashDirection(direction) * attributes.acceleration * speedBoost * .5f,
                ForceMode.Acceleration);

        OnDash?.Invoke("IsDashing", true);
        OnDashDirection?.Invoke(direction);
        attributes.canDoDamage = true;
        attributes.canInteract = true;
        lookAtRotation.useAngleRotation = true;
        SoundMaster.PlaySound(transform.position, (int)SoundListAuto.FlyDodge);

        delayedRestore = DOVirtual.DelayedCall(dashCooldown * .5f, () =>
        {
            attributes.damagePriority = -1;
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