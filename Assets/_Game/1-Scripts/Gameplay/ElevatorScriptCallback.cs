using HoudiniEngineUnity;
using UnityEngine;

[ExecuteInEditMode]
public class ElevatorScriptCallback : MonoBehaviour
{
    private ElevatorScript _elevatorScript;

    private void OnEnable()
    {
        _elevatorScript = GetComponentInParent<ElevatorScript>();

        if (gameObject.name.Contains("backplane"))
        {
            //add a meshcollider if it doesnt have one already
            var meshCollider = GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = gameObject.AddComponent<MeshCollider>();
                meshCollider.convex = true;
            }

            //remove mesh filter and renderer if it has one
            var meshFilter = GetComponent<MeshFilter>();
            if (meshFilter != null) DestroyImmediate(meshFilter);

            var meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null) DestroyImmediate(meshRenderer);

            //set layer to 12
            gameObject.layer = 12;
        }
    }

    public void ChangedParameters()
    {
        /*
        var parent = GetComponentInParent<ElevatorScript>();
        if (parent != null)
            parent.UpdateParameters();
            */

        if (gameObject.name.Contains("Ungrouped"))
        {
            _elevatorScript = GetComponentInParent<ElevatorScript>();
            if (_elevatorScript != null)
            {
                _elevatorScript.UpdateParameters();
            }
            else
            {
                var hue_parent = GetComponentInParent<HEU_HoudiniAssetRoot>();
                //instantiate from resources
                var elevator = Instantiate(Resources.Load("prefabs/Elevator-v2")) as GameObject;
                elevator.transform.position = hue_parent.transform.position;


                hue_parent.transform.parent = elevator.transform;
                hue_parent.transform.localPosition = Vector3.zero;
                var elevatorScript = elevator.GetComponent<ElevatorScript>();
                elevatorScript.assetRoot = hue_parent;

                elevatorScript.UpdateParameters();
            }
        }
    }
}