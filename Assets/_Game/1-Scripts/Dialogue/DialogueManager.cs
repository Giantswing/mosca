using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [SerializeField] private int currentDialogueIndex;
    [SerializeField] private DialogueSO currentDialogueSO;

    [Space(15)] [SerializeField] private SmartData.SmartEvent.EventDispatcher onDialogueFinished;

    [Space(25)] [Header("References")] [SerializeField]
    private GameObject dialogueParent;

    [SerializeField] private Transform leftCharacter;
    [SerializeField] private TextMeshProUGUI leftCharacterText;
    [SerializeField] private RawImage leftCharacterImage;


    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;

        HideDialogue();
    }

    public static void ShowDialogue(DialogueSO dialogueSO)
    {
        if (instance.currentDialogueIndex != 0) return;

        instance.currentDialogueSO = dialogueSO;
        instance.currentDialogueIndex = 0;
        instance.dialogueParent.SetActive(true);
        instance.leftCharacter.gameObject.SetActive(false);

        instance.SetUpCurrentDialogue();
    }

    public void SetUpCurrentDialogue()
    {
        if (currentDialogueSO.dialogue[currentDialogueIndex].leftSide)
        {
            instance.leftCharacter.gameObject.SetActive(true);
            instance.leftCharacterText.text = currentDialogueSO.dialogue[currentDialogueIndex].dialogueText;
            instance.leftCharacterImage.texture = currentDialogueSO.dialogue[currentDialogueIndex].emotion;
        }
    }

    public static void NextDialogue()
    {
        if (instance.currentDialogueIndex < instance.currentDialogueSO.dialogue.Count - 1)
        {
            instance.currentDialogueIndex++;
            instance.SetUpCurrentDialogue();
        }
        else
        {
            HideDialogue();
            instance.StartCoroutine(instance.RestoreControlRoutine());
            instance.currentDialogueIndex = 0;
            return;
        }
    }

    private IEnumerator RestoreControlRoutine()
    {
        yield return new WaitForSeconds(1f);
        instance.onDialogueFinished.Dispatch();
    }

    public static void HideDialogue()
    {
        instance.dialogueParent.SetActive(false);
    }
}