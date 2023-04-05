using System;
using DG.Tweening;
using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeed;
    [SerializeField] private Ease ease;

    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}