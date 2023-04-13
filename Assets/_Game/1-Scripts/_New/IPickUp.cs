using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickUp
{
    Rigidbody rb { get; set; }
    Transform whoToFollow { get; set; }
    Transform whoReceivesPickup { get; set; }
    bool isPickedUp { get; set; }

    bool isFollowing { get; set; }

    void StartFollowing();
    void OnCollect();
}


public struct ItemInfo
{
    public Transform itemTransform;
    public bool isHoldable;
    public bool isThrowable;
}