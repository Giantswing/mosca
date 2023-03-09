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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DialogTrigger"))
        {
            currentDialogueTriggerEvent = other.GetComponent<DialogueTriggerEvent>();
            hasDialogueTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DialogTrigger"))
        {
            currentDialogueTriggerEvent = null;
            hasDialogueTrigger = false;
            DialogueManager.HideDialogue();
        }
    }

    public void StartDialogue(InputAction.CallbackContext context)
    {
        if (context.started)
            if (hasDialogueTrigger)
            {
                DialogueManager.ShowDialogue(currentDialogueTriggerEvent.dialogueSO);
                pM.DisablePlayer();
            }
    }

    public void NextDialogue(InputAction.CallbackContext context)
    {
        if (context.started)
            if (hasDialogueTrigger)
                DialogueManager.NextDialogue();
    }
}