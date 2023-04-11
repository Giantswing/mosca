#if UNITY_EDITOR
using UnityEditor;
using System.IO;

public class GenerateEnum
{
    [MenuItem("Tools/GenerateEnum")]
    public static void Go(string enumName, string[] enumEntries)
    {
        /*
        string enumName = "MyEnum";
        string[] enumEntries = { "Foo", "Goo", "Hoo" };
        */

        var filePathAndName =
            "Assets/_Game/1-Scripts/Enums/" + enumName + ".cs"; //The folder Scripts/Enums/ is expected to exist

        using (var streamWriter = new StreamWriter(filePathAndName))
        {
            streamWriter.WriteLine("public enum " + enumName);
            streamWriter.WriteLine("{");
            for (var i = 0; i < enumEntries.Length; i++) streamWriter.WriteLine("\t" + enumEntries[i] + ",");
            streamWriter.WriteLine("}");
        }

        AssetDatabase.Refresh();
    }
}
#endif