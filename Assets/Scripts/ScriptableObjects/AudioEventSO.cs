using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioEventSO : ScriptableObject
{
    public abstract void Play(AudioSource source);
    public abstract void Play(AudioSource source, Vector3 position);
}