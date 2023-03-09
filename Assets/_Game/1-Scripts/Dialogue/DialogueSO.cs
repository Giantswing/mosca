using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Dialogue
{
    public CharacterSO character;
    public string dialogueText;
    public Texture emotion;
    public bool leftSide;
}

[CreateAssetMenu(fileName = "Character", menuName = "Flugi/Dialogue", order = 1)]
public class DialogueSO : ScriptableObject
{
    public List<Dialogue> dialogue;
}