using System;
using UnityEngine;

public class InteractablePoint : MonoBehaviour, IInteractableWithDash
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, .5f);
    }
}