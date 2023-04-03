using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerReceiveInput : MonoBehaviour
{
    public Action OnDash;
    public Action<Vector2> OnMove;
    public Vector2 inputDirectionTo;


    private void Start()
    {
    }

    public void InputMove(InputAction.CallbackContext context)
    {
        if (!enabled) return;
        inputDirectionTo = context.ReadValue<Vector2>();
        OnMove?.Invoke(inputDirectionTo);
    }

    public void InputDash(InputAction.CallbackContext context)
    {
        if (!enabled) return;
        if (context.performed) OnDash?.Invoke();
    }
}