using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAudioManager : MonoBehaviour
{
    [SerializeField] private int _maxAudioSources = 10;
    [SerializeField] private AudioSource[] _audioSources;

    public static GlobalAudioManager Instance;

    private void Awake()
    {
        Instance = this;
        _audioSources = new AudioSource[_maxAudioSources];

        for (var i = 0; i < _maxAudioSources; i++)
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            _audioSources[i] = audioSource;
        }
    }

    public static void PlaySound(AudioEventSO audioEvent, Vector3 audioPosition)
    {
        var freeAudioSource = Instance.ReturnFreeAudioSource();
        if (freeAudioSource == -1) return;
        audioEvent.Play(Instance._audioSources[freeAudioSource]);
    }

    private int ReturnFreeAudioSource()
    {
        for (var i = 0; i < _maxAudioSources; i++)
            if (!_audioSources[i].isPlaying)
                return i;

        return -1;
    }
}