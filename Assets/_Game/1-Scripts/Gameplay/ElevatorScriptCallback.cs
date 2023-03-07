using HoudiniEngineUnity;
using UnityEngine;

[ExecuteInEditMode]
public class ElevatorScriptCallback : MonoBehaviour
{
    private ElevatorScript _elevatorScript;

    private void OnEnable()
    {
        _elevatorScript = GetComponentInParent<ElevatorScript>();
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