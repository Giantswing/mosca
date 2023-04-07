using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StandardSound
{
    public string name;
    public SimpleAudioEvent audioEvent;
    public int count;
    [HideInInspector] public float duration;
}