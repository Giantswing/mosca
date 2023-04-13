using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data", menuName = "Scriptable Objects/Player Data")]
public class PlayerDataSO : ScriptableObject
{
    public Attributes attributes;
}