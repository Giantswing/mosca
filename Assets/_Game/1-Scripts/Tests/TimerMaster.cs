using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerMaster : MonoBehaviour
{
    public static Action tickFrame;
    public static Action tickEverySecondFrame;

    private void Update()
    {
        tickFrame?.Invoke();

        if (Time.frameCount % 2 == 0)
            tickEverySecondFrame?.Invoke();
    }
}