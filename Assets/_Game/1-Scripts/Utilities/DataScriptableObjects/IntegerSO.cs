using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Integer", menuName = "Scriptable Objects/Integer")]
public class IntegerSO : ScriptableObject
{
    public int value;
    public List<UnityEvent> OnValueChange = new();

    public void SetValue(int value)
    {
        this.value = value;
        OnValueChange.ForEach(e => e.Invoke());
    }
}