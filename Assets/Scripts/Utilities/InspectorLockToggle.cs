using System;
using System.Reflection;
using UnityEditor;

public class InspectorLockToggle
{
#if UNITY_EDITOR
    [MenuItem("Tools/Toggle Lock #w")]
    private static void ToggleInspectorLock() // Inspector must be inspecting something to be locked
    {
        //get current inspector window
        var inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        var inspectorToBeLocked = EditorWindow.GetWindow(inspectorType);
        //var inspectorToBeLocked = EditorWindow.GetWindow(); // "EditorWindow.focusedWindow" can be used instead

        if (inspectorToBeLocked != null && inspectorToBeLocked.GetType().Name == "InspectorWindow")
        {
            var type = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.InspectorWindow");
            var propertyInfo = type.GetProperty("isLocked");
            var value = (bool)propertyInfo.GetValue(inspectorToBeLocked, null);
            propertyInfo.SetValue(inspectorToBeLocked, !value, null);
            inspectorToBeLocked.Repaint();
        }
    }
#endif
}