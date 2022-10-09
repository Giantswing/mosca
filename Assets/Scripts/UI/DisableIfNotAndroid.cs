using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableIfNotAndroid : MonoBehaviour
{
    private void Start()
    {
        if (Application.platform != RuntimePlatform.Android) gameObject.SetActive(false);
    }
}