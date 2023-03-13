using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Emotions
{
    public string emotionName;
    public Texture emotionSprite;
}

[CreateAssetMenu(fileName = "Character", menuName = "Flugi/Character", order = 1)]
public class CharacterSO : ScriptableObject
{
    public string characterName;
    public List<Emotions> emotions;
    public SimpleAudioEvent talkSound;
}