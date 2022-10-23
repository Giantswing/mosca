using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

[Serializable]
public class MaterialFolder
{
    public string name;
    public Material material;
}

[CreateAssetMenu(fileName = "AutoMaterialOrganizer", menuName = "Utilites/AutoMaterialOrganizer", order = 1)]
public class AutoApplyMaterials : ScriptableObject
{
#if UNITY_EDITOR
    public MaterialFolder[] materialFolders;
    public PhysicMaterial defaultPhysicMaterial;
    [HideInInspector] public static AutoApplyMaterials Instance;


    private void OnEnable()
    {
        Instance = this;
    }

    [MenuItem("Tools/Auto Apply Materials #t")]
    private static void ApplyMaterials() // Inspector must be inspecting something to be locked
    {
        var allRenderers = FindObjectsOfType<MeshRenderer>(true);
        var dynamic = false;

        foreach (var renderer in allRenderers)
        {
            var found = false;
            var materials = renderer.sharedMaterials;
            dynamic = false;


            for (var i = 0; i < materials.Length; i++)
            {
                var material = materials[i];
                if (material == null) continue;

                Debug.Log(material.name);

                if (material.name == "TriplanarStatic") dynamic = true;

                if (dynamic) Debug.Log("found dynamic");

                for (var j = 0; j < Instance.materialFolders.Length; j++)
                    if (material.name == Instance.materialFolders[j].name ||
                        material.name == Instance.materialFolders[j].material.name)
                    {
                        material = Instance.materialFolders[j].material;
                        found = true;
                    }

                materials[i] = material;
            }

            if (found)
            {
                renderer.sharedMaterials = materials;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

                var meshCollider = renderer.gameObject.GetComponent<MeshCollider>();
                if (meshCollider == null) renderer.gameObject.AddComponent<MeshCollider>();

                meshCollider.material = Instance.defaultPhysicMaterial;
                meshCollider.convex = false;

                renderer.gameObject.layer = 7;

                renderer.gameObject.isStatic = !dynamic;
            }
        }

        Debug.Log("Materials applied");
    }
#endif
}