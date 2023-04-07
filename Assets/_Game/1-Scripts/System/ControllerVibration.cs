using System;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public static class ControllerVibration
{
    public static float oldStrength = 0;

    public static void Vibrate(float duration, float strength)
    {
        //if (Math.Abs(oldStrength - strength) < 0.01) return;
        Gamepad.current?.SetMotorSpeeds(strength, strength);
        DOVirtual.DelayedCall(duration, () => StopVibration());

        //oldStrength = strength;
    }

    public static void VibrateImmediate(float strength)
    {
        //if (Math.Abs(oldStrength - strength) < 0.01) return;
        Gamepad.current?.SetMotorSpeeds(strength, strength);

        //oldStrength = strength;
    }

    public static void StopVibration()
    {
        Gamepad.current?.SetMotorSpeeds(0, 0);
        Gamepad.current?.ResetHaptics();
    }
}