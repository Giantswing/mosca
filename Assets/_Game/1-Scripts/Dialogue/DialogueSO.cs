using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Chat
{
    public CharacterSO character;
    public string dialogueText;
    public int emotionIndex;
    public bool leftSide;
}


[CreateAssetMenu(fileName = "Dialogue", menuName = "Flugi/Dialogue", order = 1)]
public class DialogueSO : ScriptableObject
{
    public List<Chat> dialogueList;
    public Texture startingLeftEmotion;
    public Texture startingRightEmotion;
}