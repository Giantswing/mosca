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


[CreateAssetMenu(fileName = "SceneOrganizer", menuName = "Utilites", order = 1)]
public class OrganizeScene : ScriptableObject
{
#if UNITY_EDITOR
    [HideInInspector] public static OrganizeScene Instance;
    public OrganizeFolder[] organizeFolders;


    private void OnEnable()
    {
        Instance = this;
    }

    [MenuItem("Tools/Organize Scene #o")]
    private static void Organize()
    {
        var allGameObjects = FindObjectsOfType<GameObject>(true);

        if (allGameObjects.Length == 0)
        {
            Debug.Log("No objects selected");
            return;
        }

        var folders = new GameObject[Instance.organizeFolders.Length];

        for (var index = 0; index < Instance.organizeFolders.Length; index++)
        {
            var foundFolder = false;
            foreach (var obj in allGameObjects)
                if (obj.name == Instance.organizeFolders[index].name)
                {
                    folders[index] = obj;
                    foundFolder = true;
                    break;
                }

            if (!foundFolder)
            {
                var folder = Instance.organizeFolders[index];
                folders[index] = new GameObject(folder.name);
                folders[index].transform.position = Vector3.zero;
                folders[index].transform.rotation = Quaternion.identity;
                folders[index].transform.localScale = Vector3.one;
            }
        }

        //Organize by dynamic names (prefab names)

        foreach (var selectedObj in allGameObjects) //cycle through all objects
            for (var i = 0; i < Instance.organizeFolders.Length; i++) //cycle through all folders
                foreach (var obj in Instance.organizeFolders[i].objects) //cycle through all objects in the folder
                    if (selectedObj.name.Contains(obj.name))
                        selectedObj.transform.parent = folders[i].transform;

        //Organize by static names (names in the inspector)

        foreach (var selectedObj in allGameObjects) //cycle through all objects
            for (var i = 0; i < Instance.organizeFolders.Length; i++) //cycle through all folders
                foreach (var obj in Instance.organizeFolders[i].objectsNames) //cycle through all objects in the folder
                    if (selectedObj.name.Contains(obj))
                        selectedObj.transform.parent = folders[i].transform;
    }

#endif
}