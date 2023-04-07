using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StandardFX
{
    public string name;
    public GameObject FX;
    public float duration;
    public int maxCount;
    public Stack<GameObject> instances = new();
}