using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DialogueTriggerEvent : MonoBehaviour
{
    public DialogueSO[] dialogueSO;
    [SerializeField] private Transform prompt;
    [SerializeField] private GameObject promptButton;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private SmartData.SmartInt.IntReader playerController; //1 keyboard 2 xbox 3 switch 4 ps4 5 ps5
    [SerializeField] private InputActionAsset inputActionAsset;

    [SerializeField] private UnityEvent onEnterTrigger;
    [SerializeField] private UnityEvent onOutsideScreen;

    public bool isInverted = false;
    public bool continueAtEndOfDialogue = false;
    public int currentDialogueIndex = 0;
    [SerializeField] private List<Transform> objectsToDestroyOrCollect;
    private bool _hasObjectsToDestroyOrCollect = false;
    private WaitForSeconds checkWait = new(0.4f);

    [Space(35)] [SerializeField] private UnityEvent onConditionMet;

    private void Start()
    {
        prompt.gameObject.SetActive(false);
        if (objectsToDestroyOrCollect != null && objectsToDestroyOrCollect.Count > 0)
        {
            _hasObjectsToDestroyOrCollect = true;
            StartCoroutine(Coroutine_CheckObjectsToDestroyOrCollect());
        }
    }

    private IEnumerator Coroutine_CheckObjectsToDestroyOrCollect()
    {
        yield return checkWait;

        foreach (var obj in objectsToDestroyOrCollect)
            if (obj == null)
            {
                objectsToDestroyOrCollect.Remove(obj);
                break;
            }


        if (objectsToDestroyOrCollect.Count > 0 && _hasObjectsToDestroyOrCollect)
        {
            StartCoroutine(Coroutine_CheckObjectsToDestroyOrCollect());
        }
        else
        {
            _hasObjectsToDestroyOrCollect = false;
            onConditionMet.Invoke();
        }
    }

    public void IncreaseDialogueIndex()
    {
        if (currentDialogueIndex < dialogueSO.Length - 1)
            currentDialogueIndex++;
    }

    public void SetDialogueIndex(int index)
    {
        currentDialogueIndex = index;
    }

    public void ShowPrompt()
    {
        prompt.DOKill();
        prompt.gameObject.SetActive(true);
        prompt.localScale = Vector3.zero;
        prompt.DOScale(1, 0.6f).SetEase(Ease.OutBounce);
        onEnterTrigger.Invoke();
    }

    public void OnBecameInvisible()
    {
        onOutsideScreen.Invoke();
    }

    public void HidePrompt()
    {
        prompt.DOScale(0, 0.6f).SetEase(Ease.InElastic).OnComplete(() => prompt.gameObject.SetActive(false));
    }
}