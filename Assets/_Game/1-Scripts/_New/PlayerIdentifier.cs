using System;
using UnityEngine;

[RequireComponent(typeof(Attributes))]
public class PlayerIdentifier : MonoBehaviour
{
    [SerializeField] private PlayerDataSO playerReference;

    private void Awake()
    {
        playerReference.attributes = GetComponent<Attributes>();
    }
}