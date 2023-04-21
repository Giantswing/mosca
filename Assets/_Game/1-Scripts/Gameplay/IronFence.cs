using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class IronFence : MonoBehaviour
{
    [SerializeField] private Transform door;
    public bool isOpen = false;

    private void Start()
    {
        door = transform.GetChild(0);

        Vector3 rot = door.transform.localRotation.eulerAngles;
        if (rot.y == 0)
            isOpen = false;
        else
            isOpen = true;
    }

    public void Toggle()
    {
        if (!isOpen)
        {
            door.DOKill();
            door.DOLocalRotate(Vector3.zero, 0.3f);
            isOpen = true;
        }
        else
        {
            door.DOKill();
            door.DOLocalRotate(new Vector3(0, 90f, 0), 0.3f);
            isOpen = false;
        }
    }
}