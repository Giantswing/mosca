using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "AutoParent", menuName = "Utilites/AutoParent", order = 1)]
public class AutoParenter : ScriptableObject
{
#if UNITY_EDITOR

    private static GameObject activeObject;

    [MenuItem("Tools/Auto Parent #q")]
    private static void AutoParent()
    {
        //get current selected object
        activeObject = Selection.activeGameObject;

        //get all selected objects
        var selectedObjects = Selection.gameObjects;

        //if no object is selected, return
        if (activeObject == null)
            return;

        //for each selected object, parent them to the active one
        foreach (var selectedObject in selectedObjects)
            if (selectedObject != activeObject)
                selectedObject.transform.parent = activeObject.transform;
    }


    [MenuItem("Tools/Remove Parent ^q")]
    private static void RemoveParent()
    {
        //get current selected object
        activeObject = Selection.activeGameObject;

        //get all selected objects
        var selectedObjects = Selection.gameObjects;

        //if no object is selected, return
        if (activeObject == null)
            return;

        //for each selected object, parent them to the active one
        foreach (var selectedObject in selectedObjects)
            selectedObject.transform.parent = null;
    }

#endif
}