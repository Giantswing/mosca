using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Emotion
{
    public string name;
    public Texture sprite;
}


[CreateAssetMenu(fileName = "Character", menuName = "Flugi/Character", order = 1)]
public class CharacterSO : ScriptableObject
{
    //enum for emotions
    public string characterName;
    public List<Emotion> emotions;

    public SimpleAudioEvent talkSound;
}