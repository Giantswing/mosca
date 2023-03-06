using HoudiniEngineUnity;
using UnityEngine;

[ExecuteInEditMode]
public class ElevatorScript : MonoBehaviour
{
    [SerializeField] public float xSize, ySize;
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private ElevatorScriptDoor[] doors;
    private HEU_HoudiniAssetRoot _assetRoot;

    private void OnEnable()
    {
        _assetRoot = GetComponentInChildren<HEU_HoudiniAssetRoot>();
        if (_assetRoot != null)
            _assetRoot.HoudiniAsset.RequestCook();


        var expectedDoors = GetComponentsInChildren<ElevatorScriptDoor>();
        if (expectedDoors.Length == 0)
        {
            if (doorPrefab != null)
            {
                doors = new ElevatorScriptDoor[2];
                doors[0] = Instantiate(doorPrefab, transform).GetComponent<ElevatorScriptDoor>();
                doors[1] = Instantiate(doorPrefab, transform).GetComponent<ElevatorScriptDoor>();
                doors[0].transform.position = transform.position;
                doors[1].transform.position = transform.position;
            }
        }
        else
        {
            doors = new ElevatorScriptDoor[2];
            doors[0] = expectedDoors[0];
            doors[1] = expectedDoors[1];
        }
    }

    public void Test()
    {
        print("this is a callback");
        var parameters = _assetRoot.HoudiniAsset.Parameters.GetParameters();
        foreach (var parameter in parameters)
        {
            if (parameter._name == "sizeWidth")
                xSize = parameter._floatValues[0];
            if (parameter._name == "sizeHeight")
                ySize = parameter._floatValues[0];
        }

        doors[0].transform.position = transform.position + new Vector3(-4.1f - xSize * 10f, -2.9f, 0);
        doors[1].transform.position = transform.position + new Vector3(4.1f + xSize * 10f, -2.9f, 0);
    }
}