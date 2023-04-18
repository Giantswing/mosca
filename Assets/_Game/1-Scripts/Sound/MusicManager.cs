using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    public AudioSource musicSource;

    private void Awake()
    {
        InitializeComponents();
    }


    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            BeginMusic();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //if when the level loads it finds another music manager
            //warns the other one to change the song, then destroys itself

            if (MusicHolder.GetSong() != instance.musicSource.clip)
            {
                print("changing clip");
                instance.ChangeSong(MusicHolder.GetSong(), MusicHolder.GetVolume());
            }
            else
            {
                print("same clip, only changing volume");
                instance.ChangeVolume(MusicHolder.GetVolume());
            }

            Destroy(gameObject);
        }
    }

    private void InitializeComponents()
    {
        musicSource = GetComponent<AudioSource>();
    }

    public void ChangeVolume(float newVolume)
    {
        DOTween.To(() => musicSource.volume, x => musicSource.volume = x, newVolume, 0.5f);
    }

    public void BeginMusic()
    {
        musicSource.clip = MusicHolder.GetSong();
        musicSource.Play();
        musicSource.volume = 0;
        DOTween.To(() => musicSource.volume, x => musicSource.volume = x, MusicHolder.GetVolume(), 1f);
    }

    public void ChangeSong(AudioClip newSong, float VolumeTo)
    {
        DOTween.To(() => musicSource.volume, x => musicSource.volume = x, 0, 0.5f).onComplete += () =>
        {
            musicSource.clip = newSong;
            musicSource.Play();
            DOTween.To(() => musicSource.volume, x => musicSource.volume = x, VolumeTo, 0.5f);
        };
    }
}