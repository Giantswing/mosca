using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObjectSubscribe : MonoBehaviour
{
    private void OnEnable()
    {
        TimerMaster.tickFrame += DoSomething;
    }

    private void OnDisable()
    {
        TimerMaster.tickFrame -= DoSomething;
    }

    private void DoSomething()
    {
        for (var i = 0; i < 100; i++)
        {
            // Perform a heavy computation
            float result = Mathf.Sin(i) * Mathf.Cos(i);
        }
    }
}