using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(SceneField))]
public class SceneFieldEditor : PropertyDrawer
{
    // Scene in build data:
    private const float sceneInBuildSeparationLeft = 1;
    private const float sceneInBuildSeparationRight = 10;
    private const float sceneInBuildSeparationTotal = sceneInBuildSeparationLeft + sceneInBuildSeparationRight;

    private GUIContent sceneInBuildYesContent = new("In build");
    private GUIContent sceneInBuildNoContent = new("Not in build");
    private GUIContent sceneInBuildUnassignedContent = new("Unassigned");
    private GUIContent sceneInBuildMultipleContent = new("—");

    private GUIStyle _sceneInBuildStyle;

    private GUIStyle SceneInBuildStyle =>
        _sceneInBuildStyle ?? (_sceneInBuildStyle = new GUIStyle(EditorStyles.miniLabel));

    private float _buildIndexWidth;

    private float BuildIndexWidth
    {
        get
        {
            if (_buildIndexWidth == 0)
                SceneInBuildStyle.CalcMinMaxWidth(sceneInBuildNoContent, out _buildIndexWidth, out _);

            return _buildIndexWidth;
        }
    }


    // Scene is required data:
    private GUIContent sceneIsRequiredContent =
        new("Required", "Logs an error and fails the build if the scene is not added to builds");

    private GUIStyle _sceneIsRequiredStyleNormal;

    private GUIStyle SceneIsRequiredStyleNormal => _sceneIsRequiredStyleNormal ??
                                                   (_sceneIsRequiredStyleNormal = new GUIStyle(EditorStyles.miniLabel));

    private GUIStyle _sceneIsRequiredStylePrefabOverride;

    private GUIStyle SceneIsRequiredStylePrefabOverride => _sceneIsRequiredStylePrefabOverride ??
                                                           (_sceneIsRequiredStylePrefabOverride =
                                                               new GUIStyle(EditorStyles.miniBoldLabel));

    private float _sceneIsRequiredWidth;

    private float SceneIsRequiredWidth
    {
        get
        {
            if (_sceneIsRequiredWidth == 0)
            {
                SceneIsRequiredStylePrefabOverride.CalcMinMaxWidth(sceneIsRequiredContent, out var min, out _);
                _sceneIsRequiredWidth = min;

                EditorStyles.toggle.CalcMinMaxWidth(GUIContent.none, out min, out _);
                _sceneIsRequiredWidth += min;
            }

            return _sceneIsRequiredWidth;
        }
    }


    /// <summary>
    /// Implementation of <see cref="PropertyDrawer.OnGUI(Rect, SerializedProperty, GUIContent)"/>.
    /// </summary>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var sceneAssetProp = property.FindPropertyRelative("sceneAsset");
        var buildIndexProp = property.FindPropertyRelative("buildIndex");
        var requiredProp = property.FindPropertyRelative("required");

        position.height = EditorGUIUtility.singleLineHeight;


        // Scene asset:
        position.width -= BuildIndexWidth + sceneInBuildSeparationTotal + SceneIsRequiredWidth;

        using (new EditorGUI.PropertyScope(position, label, sceneAssetProp))
        {
            EditorGUI.PropertyField(position, sceneAssetProp, label);
        }


        // Is the scene in builds?:
        position.x += position.width + sceneInBuildSeparationLeft;
        position.width = BuildIndexWidth + sceneInBuildSeparationRight;

        if (sceneAssetProp.hasMultipleDifferentValues)
        {
            GUI.Label(position, sceneInBuildMultipleContent, SceneInBuildStyle);
        }
        else if (sceneAssetProp.objectReferenceValue != null)
        {
            var isInBuilds = buildIndexProp.intValue >= 0;

            var prevColor = GUI.contentColor;
            if (!isInBuilds && requiredProp.boolValue)
                GUI.contentColor *= Color.red;

            GUI.Label(position, isInBuilds ? sceneInBuildYesContent : sceneInBuildNoContent, SceneInBuildStyle);

            GUI.contentColor = prevColor;
        }
        else if (requiredProp.boolValue)
        {
            var prevColor = GUI.contentColor;
            GUI.contentColor *= Color.red;
            GUI.Label(position, sceneInBuildUnassignedContent, SceneInBuildStyle);
            GUI.contentColor = prevColor;
        }


        // Scene required:
        position.x += position.width;
        position.width = SceneIsRequiredWidth;

        using (new EditorGUI.PropertyScope(position, sceneIsRequiredContent, requiredProp))
        using (new EditorGUI.IndentLevelScope(-EditorGUI.indentLevel))
        using (var changeCheck = new EditorGUI.ChangeCheckScope())
        {
            EditorGUI.showMixedValue = requiredProp.hasMultipleDifferentValues;
            var newValue = EditorGUI.ToggleLeft(position, sceneIsRequiredContent, requiredProp.boolValue,
                requiredProp.prefabOverride && !requiredProp.hasMultipleDifferentValues
                    ? SceneIsRequiredStylePrefabOverride
                    : SceneIsRequiredStyleNormal);
            EditorGUI.showMixedValue = false;

            if (changeCheck.changed)
                requiredProp.boolValue = newValue;
        }
    }
}