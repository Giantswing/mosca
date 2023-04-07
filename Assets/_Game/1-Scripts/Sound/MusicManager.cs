using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    public AudioSource musicSource;
    public AudioClip currentSong;
    public MusicHolder musicHolder;

    [Range(0, 1)] public float musicVolume;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitializeComponents();
            BeginMusic();
        }
        else
        {
            var newInstance = this;
            //print("Music manager (" + newInstance.gameObject.name + ") found");

            var newSong = newInstance.currentSong;
            var newVolume = newInstance.musicVolume;
            InitializeComponents();

            if (instance.currentSong.name != newSong.name) instance.ChangeSong(newSong, newVolume);
            else if (instance.musicVolume != newVolume) instance.ChangeVolume(newVolume);

            //Destroy(newInstance.gameObject);
            var newObject = new GameObject();
            newInstance.gameObject.transform.parent = newObject.transform;
            newInstance.gameObject.GetComponent<AudioSource>().enabled = false;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void InitializeComponents()
    {
        musicSource = GetComponent<AudioSource>();
    }

    public void ChangeVolume(float newVolume)
    {
        musicVolume = newVolume;
        DOTween.To(() => musicSource.volume, x => musicSource.volume = x, musicVolume, 0.5f);
    }

    public void BeginMusic()
    {
        musicSource.clip = currentSong;
        musicSource.Play();
        musicSource.volume = 0;
        DOTween.To(() => musicSource.volume, x => musicSource.volume = x, musicVolume, 1f);
    }

    public void ChangeSong(AudioClip newSong, float VolumeTo)
    {
        DOTween.To(() => musicSource.volume, x => musicSource.volume = x, 0, 0.5f).onComplete += () =>
        {
            currentSong = newSong;
            musicSource.clip = newSong;
            musicSource.Play();
            DOTween.To(() => musicSource.volume, x => musicSource.volume = x, VolumeTo, 0.5f);
        };
    }
}