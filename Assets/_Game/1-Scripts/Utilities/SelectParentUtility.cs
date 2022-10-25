using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SelectParentUtility : EditorWindow
{
    [MenuItem("Edit/Select Parent &c")]
    private static void SelectParentOfObjects()
    {
        var newSelection = new List<GameObject>();
        foreach (var s in Selection.objects) newSelection.Add((s as GameObject).transform.parent?.gameObject);
        Selection.objects = newSelection.ToArray();
    }
}