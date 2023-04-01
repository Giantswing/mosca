using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[Serializable]
public class OrganizeFolder
{
    public string name;
    public List<GameObject> objects;
    public List<string> objectsNames;
}


[CreateAssetMenu(fileName = "SceneOrganizer", menuName = "Utilites/SceneOrganizer", order = 1)]
public class OrganizeScene : ScriptableObject
{
#if UNITY_EDITOR
    [HideInInspector] public static OrganizeScene Instance;
    public OrganizeFolder[] organizeFolders;
    public List<GameObject> allGameObjects = new();


    private void OnEnable()
    {
        Instance = this;
        ResetGameListDebug();
    }

    public void ResetGameListDebug()
    {
        allGameObjects.Clear();
    }

    [MenuItem("Tools/Organize Scene #o")]
    private static void Organize()
    {
        Instance.allGameObjects.Clear();
        var allTransformsInScene = FindObjectsOfType<Transform>(true);

        //remove parent of all objects to folder
        foreach (var folder in Instance.organizeFolders)
        foreach (var transform in allTransformsInScene)
            if (transform.parent != null && transform.parent.name == folder.name)
                transform.parent = null;
        //-----------------------------------------------------

        foreach (var transform in allTransformsInScene)
            if (transform.parent == null)
            {
                Instance.allGameObjects.Add(transform.gameObject);
                Debug.Log("Added: " + transform.gameObject.name);
            }

        if (Instance.allGameObjects.Count == 0)
        {
            Debug.Log("No objects in the scene!");
            return;
        }

        var folders = new GameObject[Instance.organizeFolders.Length];

        for (var index = 0; index < Instance.organizeFolders.Length; index++)
        {
            var foundFolder = false;
            foreach (var obj in Instance.allGameObjects)
                if (obj.name == Instance.organizeFolders[index].name)
                {
                    folders[index] = obj;
                    foundFolder = true;
                    break;
                }

            if (!foundFolder)
            {
                var folder = Instance.organizeFolders[index];
                folders[index] = new GameObject(folder.name)
                {
                    transform =
                    {
                        position = Vector3.zero,
                        rotation = Quaternion.identity,
                        localScale = Vector3.one
                    }
                };
            }

            Instance.allGameObjects.Remove(folders[index]);
            Debug.Log("Removed folder: " + folders[index].name);
        }


        foreach (var selectedObj in Instance.allGameObjects) //cycle through all objects
            for (var i = 0; i < Instance.organizeFolders.Length; i++) //cycle through all folders
                foreach (var folderObj in Instance.organizeFolders[i].objects) //cycle through all objects in the folder
                    if (selectedObj.name.Contains(folderObj.name))
                    {
                        //check if selectedObj has a mover parent
                        if (selectedObj.transform.parent != null)
                            if (selectedObj.transform.parent.name.Contains("Mover") ||
                                selectedObj.transform.parent.name.Contains("Cog6") ||
                                selectedObj.transform.parent.name.Contains("DSwitcher") ||
                                selectedObj.transform.parent.name.Contains("Elevator") ||
                                selectedObj.transform.parent.name.Contains("Traveler"))
                                break;

                        selectedObj.transform.parent = folders[i].transform;
                    }

        //Organize by static names (names in the inspector)

        foreach (var selectedObj in Instance.allGameObjects) //cycle through all objects
            for (var i = 0; i < Instance.organizeFolders.Length; i++) //cycle through all folders
                foreach (var obj in Instance.organizeFolders[i].objectsNames) //cycle through all objects in the folder
                    if (selectedObj.name.Contains(obj))
                        selectedObj.transform.parent = folders[i].transform;


        var assembly = Assembly.GetAssembly(typeof(ActiveEditorTracker));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }

#endif
}