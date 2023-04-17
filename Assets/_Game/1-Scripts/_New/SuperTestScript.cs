using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SuperTestScript : MonoBehaviour
{
    [SerializeField] private AreaGetter myAreaGetter;
    private bool growMode = true;
    private float size = 0;

    private void Start()
    {
        InvokeRepeating(nameof(Reset), 0, 3f);
    }

    private void Update()
    {
        myAreaGetter.UpdateAreaSize(Time.deltaTime * 3f);
    }

    private void Reset()
    {
        size = 0.1f;
        myAreaGetter.ResetArea();
    }
}