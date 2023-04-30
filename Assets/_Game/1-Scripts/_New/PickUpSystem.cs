using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(ItemHolder))]
public class PickUpSystem : MonoBehaviour
{
    private ItemHolder itemHolder;
    public PickUpSystem parentPickupSystem;
    [SerializeField] private Collider[] colliders = new Collider[10];
    [SerializeField] private float pickupRange = 1f;


    private void Awake()
    {
        itemHolder = GetComponent<ItemHolder>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(CheckForPickups), 0, .05f);
    }

    private void CheckForPickups()
    {
        if (!enabled) return;

        int numOverlaps =
            Physics.OverlapSphereNonAlloc(transform.position, pickupRange, colliders, int.MaxValue,
                QueryTriggerInteraction.Collide);

        for (var i = 0; i < numOverlaps; i++)
            if (colliders[i].TryGetComponent(out IPickUp pickUp))
                GrabPickup(colliders[i], pickUp);
    }

    private void GrabPickup(Collider other, IPickUp pickUp)
    {
        if (pickUp.isPickedUp) return;

        bool notBlocked = !Physics.Raycast(transform.position, other.transform.position - transform.position,
            out RaycastHit hit, pickupRange);

        if (notBlocked)
            Debug.DrawRay(transform.position, other.transform.position - transform.position, Color.green, 3f);
        else
            Debug.DrawRay(transform.position, other.transform.position - transform.position, Color.red, 3f);

        if (!notBlocked) return;


        pickUp.isPickedUp = true;
        pickUp.whoToFollow = transform;
        pickUp.whoReceivesPickup = parentPickupSystem == null ? transform : parentPickupSystem.transform;

        //PickUp sequence, first gets pushed away for a moment
        Vector3 oppositeDirection = (pickUp.rb.position - transform.position).normalized;
        pickUp.rb.AddForce(oppositeDirection * 12f, ForceMode.Impulse);

        DOVirtual.DelayedCall(Random.Range(0.25f, 0.4f), () => { pickUp.StartFollowing(); });
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}