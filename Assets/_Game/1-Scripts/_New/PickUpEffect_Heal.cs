using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpEffect_Heal : MonoBehaviour, IPickUpEffect
{
    public int healValue = 1;

    public void OnCollect(Transform target)
    {
        if (target.TryGetComponent(out Attributes attributes)) attributes.Heal(healValue);
    }
}