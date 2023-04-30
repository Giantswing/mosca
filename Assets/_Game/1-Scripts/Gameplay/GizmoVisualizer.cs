using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class GizmoVisualizer : MonoBehaviour
{
    public Color gizmoColor = Color.yellow; // Color of the gizmo box
    public bool showInPlayMode = false; // Whether to show the gizmo in Play Mode

    private void OnDrawGizmos()
    {
        if (!showInPlayMode && Application.isPlaying)
            return;

#if UNITY_EDITOR
        // Get the object's bounds in world space
        Bounds bounds = GetBounds();

        // Draw the gizmo box
        Handles.matrix = transform.localToWorldMatrix; // Apply object's rotation and scale
        Handles.color = gizmoColor;
        Handles.DrawWireCube(bounds.center, bounds.size);
        Handles.matrix = Matrix4x4.identity; // Reset matrix
#endif
    }

    private Bounds GetBounds()
    {
        // Get the object's renderer (if available)
        Renderer renderer = GetComponent<Renderer>();

        // If renderer is null, use collider bounds as fallback
        if (renderer == null)
        {
            Collider collider = GetComponent<Collider>();
            if (collider != null)
                return collider.bounds;
        }

        // If renderer is available, use its bounds
        if (renderer != null)
            return renderer.bounds;

        // If neither renderer nor collider is available, use a default bounds
        return new Bounds(transform.position, Vector3.one);
    }
}