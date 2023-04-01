using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDialogueHandler : MonoBehaviour
{
    [SerializeField] private DialogueTriggerEvent currentDialogueTriggerEvent;
    [SerializeField] private PlayerMovement pM;
    private bool hasDialogueTrigger = false;
    private bool isDialogueActive = false;
    private DialogueManager DialogueManager;

    private void Start()
    {
        DialogueManager = DialogueManager.GetInstance();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DialogTrigger"))
        {
            currentDialogueTriggerEvent = other.GetComponent<DialogueTriggerEvent>();
            currentDialogueTriggerEvent.ShowPrompt();
            hasDialogueTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DialogTrigger"))
        {
            if (currentDialogueTriggerEvent != null) currentDialogueTriggerEvent.HidePrompt();
            currentDialogueTriggerEvent = null;
            hasDialogueTrigger = false;
            isDialogueActive = false;
            DialogueManager.HideDialogue();
        }
    }

    public void StartDialogue(InputAction.CallbackContext context)
    {
        if (context.started)
            if (hasDialogueTrigger && !isDialogueActive)
            {
                DialogueManager.isInverted = currentDialogueTriggerEvent.isInverted;
                DialogueManager.enabled = true;
                DialogueManager.ShowDialogue(
                    currentDialogueTriggerEvent.dialogueSO[currentDialogueTriggerEvent.currentDialogueIndex]);
                isDialogueActive = true;
                pM.DisablePlayer();
                currentDialogueTriggerEvent.HidePrompt();
            }
    }

    public void StopDialogue()
    {
        isDialogueActive = false;
        DialogueManager.enabled = false;
        currentDialogueTriggerEvent.ShowPrompt();
    }

    public void NextDialogue(InputAction.CallbackContext context)
    {
        if (context.started)
            if (hasDialogueTrigger && isDialogueActive)
                DialogueManager.NextDialogue();
    }
}