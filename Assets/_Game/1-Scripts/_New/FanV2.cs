using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanV2 : MonoBehaviour
{
    [SerializeField] private AreaGetter myAreaGetter;
    [SerializeField] private float force = 10f;

    private void FixedUpdate()
    {
        if (myAreaGetter.GetAreaListCount() > 0 && myAreaGetter.isClosestRigidbodyWithHardCollider())
            myAreaGetter.GetClosestRigidbodyWithHardCollider()
                .AddForce(transform.right * force, ForceMode.Acceleration);
    }
}