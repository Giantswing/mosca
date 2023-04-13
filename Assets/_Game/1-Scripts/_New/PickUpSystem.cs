using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(ItemHolder))]
public class PickUpSystem : MonoBehaviour
{
    private ItemHolder itemHolder;
    public PickUpSystem parentPickupSystem;

    private void Awake()
    {
        itemHolder = GetComponent<ItemHolder>();
    }

    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;

        if (other.TryGetComponent(out IPickUp pickUp))
        {
            if (pickUp.isPickedUp) return;

            pickUp.isPickedUp = true;
            pickUp.whoToFollow = transform;
            pickUp.whoReceivesPickup = parentPickupSystem == null ? transform : parentPickupSystem.transform;

            //PickUp sequence, first gets pushed away for a moment
            Vector3 oppositeDirection = (pickUp.rb.position - transform.position).normalized;
            pickUp.rb.AddForce(oppositeDirection * 9f, ForceMode.Impulse);

            DOVirtual.DelayedCall(.3f, () => { pickUp.StartFollowing(); });
        }
    }
}