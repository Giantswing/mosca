using System.Collections.Generic;
using UnityEngine;

public class IgnoredParentObjects
{
    public static List<string> ignoredParentObjects = new();

    private void Awake()
    {
        ignoredParentObjects.Add("Mover");
        ignoredParentObjects.Add("DSwitcher");
        ignoredParentObjects.Add("spike-blob");
    }

    public static bool shouldBeIgnored(Transform transform)
    {
        var result = false;

        if (transform.parent == null)
            return false;

        var name = transform.parent.name;

        for (var i = 0; i < ignoredParentObjects.Count; i++)
            if (name.Contains(ignoredParentObjects[i]))
            {
                result = true;
                break;
            }

        return result;
    }
}