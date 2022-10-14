using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CampaignSO))]
public class ResetAllScores : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var myScript = (CampaignSO)target;

        if (GUILayout.Button("Reset all stars")) myScript.ResetAllStars();
    }
}