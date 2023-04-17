using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;

public class InputReceiver : MonoBehaviour
{
    public UnityEvent<Vector3> OnMove;
    public UnityEvent<Vector3> OnButtonSouth;
    public UnityEvent<Vector3> OnButtonWest;
    public UnityEvent<Vector3> OnButtonWestReleased;

    public Vector3 lastInputDirection;
    private PlayerInput playerInput;
    public InputDevice inputDevice;
    public int playerIndex;


    private void OnEnable()
    {
        playerInput = GetComponent<PlayerInput>();
        playerIndex = playerInput.playerIndex;

        /*
        UpdateInputDevice();

        DOVirtual.DelayedCall(0.25f, () =>
        {
            playerInput.SwitchCurrentControlScheme(inputDevice.name, inputDevice);
            print(playerInput.devices[0]);
        });
        */
    }

    public void ForceController()
    {
        UpdateInputDevice();

        DOVirtual.DelayedCall(0.25f, () =>
        {
            playerInput.SwitchCurrentControlScheme(inputDevice.name, inputDevice);
            print(playerInput.devices[0]);
        });
    }


    public void Move(InputAction.CallbackContext context)
    {
        if (!enabled) return;
        lastInputDirection = context.ReadValue<Vector2>();
        OnMove?.Invoke(lastInputDirection);

        //UpdateInputDevice();
    }

    public void ButtonSouth(InputAction.CallbackContext context)
    {
        if (!enabled || context.canceled) return;
        OnButtonSouth?.Invoke(lastInputDirection);

        //UpdateInputDevice();
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

        //UpdateInputDevice();
    }

    /*
    public void JoinPlayerButton(InputAction.CallbackContext context)
    {
        if (!enabled || !context.canceled) return;

        //if the device pressed is the same as the current device, do nothing
        if (playerInput.devices[0] == inputDevice) return;


        DOVirtual.DelayedCall(0.1f, () =>
        {
            TargetGroupControllerSystem.JoinPlayerStatic(playerInput.devices[0]);

            playerInput.SwitchCurrentControlScheme(inputDevice.name, inputDevice);
        });
    }
    */

    public void UpdateInputDevice()
    {
        inputDevice = playerInput.devices[0];
        //print(inputDevice);
    }


    private void OnDisable()
    {
        lastInputDirection = Vector3.zero;
        OnMove?.Invoke(lastInputDirection);
    }

    /*
    private void Start()
    {
        UpdateInputDevice();

        DOVirtual.DelayedCall(0.1f, () =>
        {
            playerInput.SwitchCurrentControlScheme(inputDevice.name, inputDevice);
            print(playerInput.devices[0]);
        });
    }*/
}