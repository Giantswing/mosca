using UnityEngine;

[ExecuteInEditMode]
public class ElevatorScriptCallback : MonoBehaviour
{
    public void Yoni()
    {
        var parent = GetComponentInParent<ElevatorScript>();
        if (parent != null)
            parent.Test();
    }
}