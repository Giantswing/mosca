using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputReceiver : MonoBehaviour
{
    public UnityEvent<Vector3> OnMove;
    public UnityEvent<Vector3> OnButtonSouth;
    public UnityEvent<Vector3> OnButtonWest;
    public UnityEvent<Vector3> OnButtonWestReleased;

    public Vector3 lastInputDirection;

    public void Move(InputAction.CallbackContext context)
    {
        if (!enabled) return;
        lastInputDirection = context.ReadValue<Vector2>();
        OnMove?.Invoke(lastInputDirection);
    }

    public void ButtonSouth(InputAction.CallbackContext context)
    {
        if (!enabled || context.canceled) return;
        OnButtonSouth?.Invoke(lastInputDirection);
    }

    public void ButtonWest(InputAction.CallbackContext context)
    {
        if (!enabled) return;

        if (context.canceled)
        {
            OnButtonWestReleased?.Invoke(lastInputDirection);
            return;
        }

        OnButtonWest?.Invoke(lastInputDirection);
    }

    private void OnDisable()
    {
        lastInputDirection = Vector3.zero;
        OnMove?.Invoke(lastInputDirection);
    }

    private void Start()
    {
    }
}