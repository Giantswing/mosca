using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHolder : MonoBehaviour
{
    public AudioClip currentSong;

    [Range(0, 1)] public float musicVolume;

    public static MusicHolder instance;

    private void Awake()
    {
        instance = this;
    }

    public static AudioClip GetSong()
    {
        return instance.currentSong;
    }

    public static float GetVolume()
    {
        return instance.musicVolume;
    }
}