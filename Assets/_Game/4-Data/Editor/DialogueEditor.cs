using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(DialogueSO))]
public class DialogueSOEditor : Editor
{
    private ReorderableList dialogueList;

    private void OnEnable()
    {
        dialogueList = new ReorderableList(serializedObject, serializedObject.FindProperty("dialogueList"), true, true,
            true, true);
        dialogueList.elementHeight = EditorGUIUtility.singleLineHeight * 4 + 10;

        dialogueList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Dialogue List"); };

        dialogueList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = dialogueList.serializedProperty.GetArrayElementAtIndex(index);

            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("character"));
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + 2, rect.width,
                    EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("dialogueText"));

            //show emotion as a dropdown
            var emotion = element.FindPropertyRelative("emotionIndex");
            var character = element.FindPropertyRelative("character");
            if (character.objectReferenceValue != null)
            {
                var emotionNames = new string[((CharacterSO)character.objectReferenceValue).emotions.Count];
                for (var i = 0; i < emotionNames.Length; i++)
                    emotionNames[i] = ((CharacterSO)character.objectReferenceValue).emotions[i].name;

                emotion.intValue = EditorGUI.Popup(
                    new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight + 2) * 2, rect.width,
                        EditorGUIUtility.singleLineHeight), emotion.intValue, emotionNames);
            }
            else
            {
                emotion.intValue = -1;
            }

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight + 2) * 3, rect.width / 2,
                    EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("leftSide"));
        };

        dialogueList.onAddCallback = (ReorderableList list) =>
        {
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("character").objectReferenceValue = null;
            element.FindPropertyRelative("dialogueText").stringValue = "";
            element.FindPropertyRelative("emotionIndex").objectReferenceValue = null;
            element.FindPropertyRelative("leftSide").boolValue = false;
            serializedObject.ApplyModifiedProperties();
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        dialogueList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}