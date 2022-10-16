using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEditor;

public class ManualReloadAssembly : MonoBehaviour
{
    [MenuItem("Tools/Reload Assemblies #e")]
    private static void ManuallyReloadAssemblies() // Inspector must be inspecting something to be locked
    {
        EditorApplication.LockReloadAssemblies();
        EditorApplication.UnlockReloadAssemblies();

        /*
        
        var assembly = Assembly.GetAssembly(typeof(Editor));
        var type = assembly.GetType("UnityEditor.ScriptCompilation.EditorCompilationInterface");
        var method = type.GetMethod("RequestScriptReload", BindingFlags.Static | BindingFlags.Public);
        method.Invoke(null, null);
        */
    }
}