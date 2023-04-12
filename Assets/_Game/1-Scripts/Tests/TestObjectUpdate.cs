using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObjectUpdate : MonoBehaviour
{
    private void Update()
    {
        for (var i = 0; i < 100; i++)
        {
            // Perform a heavy computation
            float result = Mathf.Sin(i) * Mathf.Cos(i);
        }
    }
}