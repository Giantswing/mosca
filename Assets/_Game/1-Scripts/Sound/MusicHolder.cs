using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHolder : MonoBehaviour
{
    public AudioClip currentSong;

    [Range(0, 1)] public float musicVolume;
}