using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class SoundMaster : MonoBehaviour
{
    private static SoundMaster instance;
    [SerializeField] private int maxInstances;
    private Stack<AudioSource> _audioSources = new();
    [SerializeField] private List<StandardSound> soundList = new();
    [SerializeField] private float maxSoundDistance;
    private Transform soundOrigin;

    public void CreateSoundListEnumDynamically()
    {
        print("regenerating sound enum");
        //create string array with all sound names
        var SoundNames = new string[soundList.Count];

        for (var i = 0; i < soundList.Count; i++)
        {
            string soundName = soundList[i].name;
            soundName = soundName.Replace(" ", "");
            SoundNames[i] = soundName;
        }

        GenerateEnum.Go("SoundListAuto", SoundNames);
    }

    /*
    private void OnValidate()
    {
        print("Sound enum regenerated");

        CreateSoundListEnumDynamically();
    }
    */

    private void Awake()
    {
        InitializeSounds();
        instance = this;
        soundOrigin = Camera.main.transform;
    }

    private void InitializeSounds()
    {
        for (var i = 0; i < maxInstances; i++)
        {
            GameObject soundInstance = new("Sound Source " + (i + 1));
            soundInstance.transform.parent = transform;
            AudioSource audioSource = soundInstance.AddComponent<AudioSource>();
            audioSource.maxDistance = maxSoundDistance;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            _audioSources.Push(audioSource);

            soundInstance.SetActive(false);
        }

        for (var j = 0; j < soundList.Count; j++)
        {
            StandardSound sound = soundList[j];
            sound.duration = FindSoundLength(sound.audioEvent);
            sound.count = 0;
        }
    }

    public static void PlaySound(Vector3 position, int index = -1, bool usePosition = true)
    {
        if (usePosition && Vector3.Distance(position, instance.soundOrigin.position) >
            instance.maxSoundDistance) return;

        StandardSound sound = instance.FindSound(index);

        /*
        if (name != "")
            sound = instance.FindSound(name);
        else
            sound = instance.FindSound(index);
        */

        if (instance._audioSources.Count == 0 || sound == null || sound.count > 25) return;


        AudioSource soundInstance = instance._audioSources.Pop();

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
        if (usePosition && Vector3.Distance(position, instance.soundOrigin.position) >
            instance.maxSoundDistance) return;

        AudioSource soundInstance = instance._audioSources.Pop();
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
        StandardSound sound = instance.FindSound(index);
        if (sound == null) return;

        var audioSourceList = new List<AudioSource>(instance._audioSources);
        for (var i = 0; i < audioSourceList.Count; i++)
        {
            AudioSource audioSource = audioSourceList[i];
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
        StandardSound sound = instance.FindSound(index);
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

    public static SimpleAudioEvent GetAudioEvent(int audioEventIndex)
    {
        return instance.soundList[audioEventIndex].audioEvent;
    }
}

/* Custom editor for SoundMaster */
[CustomEditor(typeof(SoundMaster))]
public class SoundMasterEditor : Editor
{
    //add button
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SoundMaster myScript = (SoundMaster)target;
        if (GUILayout.Button("Generate Sound Enum"))
            myScript.CreateSoundListEnumDynamically();
    }
}