using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelSO))]
public class CountSceneScore : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var myScript = (LevelSO)target;

        //if (GUILayout.Button("Count Score")) myScript.CountScore();
    }
}