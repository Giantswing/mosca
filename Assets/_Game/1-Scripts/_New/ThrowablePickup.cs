using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class ThrowablePickup : HoldablePickup
{
    [Title("Throwable Properties")] public float throwForce = 10f;
    [HideInInspector] public Collider hardCollider;
    public UnityEvent OnThrow;

    protected override void Initialize()
    {
        base.Initialize();
        hardCollider = GetComponents<Collider>().First(c => c.isTrigger == false);
        hardCollider.enabled = false;
        isThrowable = true;
    }

    public override void Throw(Vector3 throwDirection, Collider colliderToIgnore)
    {
        rb.useGravity = true;
        if (colliderToIgnore != null)
            Physics.IgnoreCollision(hardCollider, colliderToIgnore);

        hardCollider.enabled = true;
        Release();
        rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        OnThrow?.Invoke();
    }
}