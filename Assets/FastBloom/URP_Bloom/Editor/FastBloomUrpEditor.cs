
using UnityEngine.Rendering.Universal;

namespace UnityEditor.Rendering.Universal
{
    [CustomEditor(typeof(FastBloomUrp))]
    [CanEditMultipleObjects]
    sealed class MobileBloomUrpEditor : Editor
    {
        SerializedProperty blitMaterial;
        SerializedProperty settings;
        SerializedProperty setBloomIterations;
        SerializedProperty bloomIterations;
        SerializedProperty bloomDiffusion;
        SerializedProperty bloomColor;
        SerializedProperty bloomAmount;
        SerializedProperty bloomThreshold;
        SerializedProperty bloomSoftness;
        private bool init = false;

        public void Init()
        {
            var o = new PropertyFetcher<FastBloomUrp>(serializedObject);
            settings = serializedObject.FindProperty("settings");
            blitMaterial = settings.FindPropertyRelative("blitMaterial");
            setBloomIterations = settings.FindPropertyRelative("SetBloomIterations");
            bloomIterations = settings.FindPropertyRelative("BloomIterations");
            bloomDiffusion = settings.FindPropertyRelative("BloomDiffusion");
            bloomColor = settings.FindPropertyRelative("BloomColor");
            bloomAmount = settings.FindPropertyRelative("BloomAmount");
            bloomThreshold = settings.FindPropertyRelative("BloomThreshold");
            bloomSoftness = settings.FindPropertyRelative("BloomSoftness");
        }

        public override void OnInspectorGUI()
        {
            if (!init)
                Init();
            serializedObject.Update();
            EditorGUILayout.PropertyField(blitMaterial);
            EditorGUILayout.PropertyField(setBloomIterations);
            if (setBloomIterations.boolValue)
                EditorGUILayout.PropertyField(bloomIterations);
            EditorGUILayout.PropertyField(bloomDiffusion);
            EditorGUILayout.PropertyField(bloomColor);
            EditorGUILayout.PropertyField(bloomAmount);
            EditorGUILayout.PropertyField(bloomThreshold);
            EditorGUILayout.PropertyField(bloomSoftness);
        }
    }
}