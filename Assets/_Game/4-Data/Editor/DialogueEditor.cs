using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(DialogueSO))]
public class DialogueSOEditor : Editor
{
    private ReorderableList dialogueList;
    private Texture startingLeftEmotion;
    private Texture startingRightEmotion;

    private void OnEnable()
    {
        dialogueList = new ReorderableList(serializedObject, serializedObject.FindProperty("dialogueList"), true, true,
            true, true);
        dialogueList.elementHeight =
            EditorGUIUtility.singleLineHeight * 7 + 20; // Increased the height to fit 6 lines of text

        dialogueList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Dialogue List"); };

        dialogueList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = dialogueList.serializedProperty.GetArrayElementAtIndex(index);

            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("character"));
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + 2, rect.width,
                    EditorGUIUtility.singleLineHeight * 4), // Increased the height to fit 4 lines of text
                element.FindPropertyRelative("dialogueText"), true);

            //show emotion as a dropdown
            var emotion = element.FindPropertyRelative("emotionIndex");
            var character = element.FindPropertyRelative("character");
            if (character.objectReferenceValue != null)
            {
                var emotionNames = new string[((CharacterSO)character.objectReferenceValue).emotions.Count];
                for (var i = 0; i < emotionNames.Length; i++)
                    emotionNames[i] = ((CharacterSO)character.objectReferenceValue).emotions[i].name;

                emotion.intValue = EditorGUI.Popup(
                    new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight + 2) * 5, rect.width,
                        EditorGUIUtility.singleLineHeight), emotion.intValue, emotionNames);
            }
            else
            {
                emotion.intValue = -1;
            }

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight + 2) * 6, rect.width / 2,
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

        //add callback for when the Textures are changed
        startingLeftEmotion = ((DialogueSO)target).startingLeftEmotion;
        startingRightEmotion = ((DialogueSO)target).startingRightEmotion;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ((DialogueSO)target).startingLeftEmotion = (Texture)EditorGUILayout.ObjectField("Starting Left Emotion",
            ((DialogueSO)target).startingLeftEmotion, typeof(Texture), false);
        ((DialogueSO)target).startingRightEmotion = (Texture)EditorGUILayout.ObjectField("Starting Right Emotion",
            ((DialogueSO)target).startingRightEmotion, typeof(Texture), false);

        dialogueList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();

        //set dirty so that the textures are updated
        if (startingLeftEmotion != ((DialogueSO)target).startingLeftEmotion ||
            startingRightEmotion != ((DialogueSO)target).startingRightEmotion)
            EditorUtility.SetDirty(target);
    }
}