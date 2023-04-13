using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(FlipSystem))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Attributes))]
public class ItemHolder : MonoBehaviour
{
    private FlipSystem _flipSystem;
    private Rigidbody _rb;
    private Attributes _attributes;


    private void Awake()
    {
        _flipSystem = GetComponent<FlipSystem>();
        _rb = GetComponent<Rigidbody>();
        _attributes = GetComponent<Attributes>();
    }


    public List<HoldablePickup> items = new();

    public UnityEvent onThrowableItemAdded;
    public UnityEvent onNoMoreThrowables;

    public void AddItem(HoldablePickup item)
    {
        if (!enabled) return;
        items.Add(item);

        if (item.isThrowable) onThrowableItemAdded?.Invoke();
    }

    public void RemoveItem(HoldablePickup item)
    {
        if (!enabled) return;
        items.Remove(item);

        if (items.All(i => !i.isThrowable)) onNoMoreThrowables?.Invoke();
    }

    private void Update()
    {
        for (var i = 0; i < items.Count; i++)
            if (items[i].isThrowable)
            {
                items[i].transform.position = Vector3.Lerp(items[i].transform.position,
                    transform.position + Vector3.up * .75f, Time.deltaTime * 7f);
            }
            else
            {
                Vector3 xPos = Vector3.left * .6f - Vector3.right * (1.25f * i);
                items[i].transform.position = Vector3.Lerp(items[i].transform.position,
                    transform.position + Vector3.up * 0.75f + xPos * _flipSystem.flipDirection, Time.deltaTime * 7f);
            }
    }

    public void ThrowItem()
    {
        if (!enabled) return;
        HoldablePickup currentThrowableItem = items.FirstOrDefault(item => item.isThrowable);
        if (currentThrowableItem != null)
            currentThrowableItem.Throw(_rb.velocity.normalized, _attributes.hardCollider);
    }

    public void RemoveNotThrowables()
    {
        if (!enabled) return;
        items.Where(item => !item.isThrowable).ToList().ForEach(item =>
        {
            item.Release(true);
            item.Reset();
        });
    }
}