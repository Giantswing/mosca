using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum SoundList
{
    FlyDodge,
    WallHit,
    CoinCollect,
    BombBeep,
    Explosion,
    DoorClosing,
    DoorHit,
    SecretWall,
    LockOpening,
    FlySwatterHit,
    UINotification,
    LevelPortalTransitionIn,
    LevelPortalTransitionOut,
    KeyBreak,
    InsecticideGas,
    CrownHit,
    BubbleHit,
    CrownCharge,
    CrownReturnHit,
    ElectricPowerUp,
    ElectricPowerOff,
    ReviverDetecting,
    ReviverReviving,
    CrownThrow
}

public class SoundMaster : MonoBehaviour
{
    private static SoundMaster instance;
    [SerializeField] private int maxInstances;
    private Stack<AudioSource> _audioSources = new();
    [SerializeField] private List<StandardSound> soundList = new();
    [SerializeField] private float maxSoundDistance;

    private void Awake()
    {
        InitializeSounds();
        instance = this;
    }

    private void InitializeSounds()
    {
        for (var i = 0; i < maxInstances; i++)
        {
            var soundInstance = new GameObject("Sound Source " + (i + 1));
            soundInstance.transform.parent = transform;
            var audioSource = soundInstance.AddComponent<AudioSource>();
            audioSource.maxDistance = maxSoundDistance;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            _audioSources.Push(audioSource);

            soundInstance.SetActive(false);
        }

        for (var j = 0; j < soundList.Count; j++)
        {
            var sound = soundList[j];
            sound.duration = FindSoundLength(sound.audioEvent);
            sound.count = 0;
        }
    }

    public static void PlaySound(Vector3 position, int index = -1, string name = "", bool usePosition = true)
    {
        if (usePosition && Vector3.Distance(position, PlayerMovement.ReturnPlayerTransform().position) >
            instance.maxSoundDistance) return;

        StandardSound sound;
        if (name != "")
            sound = instance.FindSound(name);
        else
            sound = instance.FindSound(index);

        if (instance._audioSources.Count == 0 || sound == null || sound.count > 25) return;


        var soundInstance = instance._audioSources.Pop();

        soundInstance.gameObject.SetActive(true);
        soundInstance.transform.position = position;
        soundInstance.spatialBlend = usePosition ? 1 : 0;
        sound.audioEvent.Play(soundInstance);
        sound.count++;

        DOVirtual.DelayedCall(sound.duration, () =>
        {
            soundInstance.Stop();
            soundInstance.gameObject.SetActive(false);
            sound.count--;
            instance._audioSources.Push(soundInstance);
        });
    }

    public static void PlayTargetSound(Vector3 position, SimpleAudioEvent sound, bool usePosition = true)
    {
        if (usePosition && Vector3.Distance(position, PlayerMovement.ReturnPlayerTransform().position) >
            instance.maxSoundDistance) return;

        var soundInstance = instance._audioSources.Pop();
        soundInstance.gameObject.SetActive(true);
        soundInstance.transform.position = position;
        soundInstance.spatialBlend = usePosition ? 1 : 0;
        sound.Play(soundInstance);

        DOVirtual.DelayedCall(FindSoundLength(sound), () =>
        {
            soundInstance.Stop();
            soundInstance.gameObject.SetActive(false);
            instance._audioSources.Push(soundInstance);
        });
    }


    public static void StopTargetSound(int index)
    {
        var sound = instance.FindSound(index);
        if (sound == null) return;

        var audioSourceList = new List<AudioSource>(instance._audioSources);
        for (var i = 0; i < audioSourceList.Count; i++)
        {
            var audioSource = audioSourceList[i];
            if (audioSource.clip == sound.audioEvent.clips[0])
            {
                print("sound stopped");
                audioSource.Stop();
                audioSource.gameObject.SetActive(false);
                instance._audioSources.Push(audioSource);
                break;
            }
        }
    }

    public static int ReturnSoundCount(int index)
    {
        var sound = instance.FindSound(index);
        if (sound == null) return 0;
        return sound.count;
    }

    public static float FindSoundLength(SimpleAudioEvent sound)
    {
        float result = 0;
        for (var x = 0; x < sound.clips.Length; x++)
            if (sound.clips[x].length > result)
                result = sound.clips[x].length;

        return result;
    }


    private StandardSound FindSound(string name)
    {
        return soundList.Find(x => x.name == name);
    }

    private StandardSound FindSound(int index)
    {
        return soundList[index];
    }

    private void OnDestroy()
    {
        instance = null;
        DOTween.KillAll();
    }
}