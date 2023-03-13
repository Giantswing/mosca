using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    [SerializeField] private UIAnimator uiAnimator;
    private int currentCharacterIndex;
    private string textToShow;
    public bool finishedWithCurrentText = false;
    private WaitForSeconds waitTime = new(0.05f);
    [SerializeField] private int currentDialogueIndex;
    [SerializeField] private DialogueSO currentDialogueSO;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SimpleAudioEvent talkSound;
    [SerializeField] private RawImage background;

    //[SerializeField] private Color backgroundColor;
    //private Color _transparentColor = new(0, 0, 0, 0);

    [Space(15)] [SerializeField] private SmartData.SmartEvent.EventDispatcher onDialogueFinished;

    [Space(25)] [Header("References")] [SerializeField]
    private GameObject dialogueParent;

    [SerializeField] private GameObject dialoguePrefab;
    [SerializeField] private CanvasGroup canvasGroup;

    private Transform[] characters = new Transform[2];
    private TextMeshProUGUI[] characterTexts = new TextMeshProUGUI[2];
    private RawImage[] characterImages = new RawImage[2];
    private Transform[] characterButtonPrompt = new Transform[2];
    private Vector3[] characterButtonPromptSize = new Vector3[2];
    private GameObject[] characterDialogueBoxes = new GameObject[2];

    private int currentCharacterReference;
    private int otherCharacterReference;


    public Coroutine TextRoutine = null;


    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;

        //HideDialogue();
        InitializeCharacterPrefabs();
        HideDialogue();
    }

    private void InitializeCharacterPrefabs()
    {
        var char1 = Instantiate(dialoguePrefab, dialogueParent.transform).GetComponent<DialogueReferences>();
        var char2 = Instantiate(dialoguePrefab, dialogueParent.transform).GetComponent<DialogueReferences>();

        characters[0] = char1.character;
        characters[1] = char2.character;

        characterTexts[0] = char1.characterText;
        characterTexts[1] = char2.characterText;

        characterImages[0] = char1.characterImage;
        characterImages[1] = char2.characterImage;

        characterButtonPrompt[0] = char1.characterButtonPrompt;
        characterButtonPrompt[1] = char2.characterButtonPrompt;

        characterDialogueBoxes[0] = char1.characterDialogueBox.gameObject;
        characterDialogueBoxes[1] = char2.characterDialogueBox.gameObject;


        var rightCharRect = char2.GetComponent<RectTransform>();
        rightCharRect.anchorMin = new Vector2(.5f, 0);
        rightCharRect.anchorMax = new Vector2(.5f, 0);
        rightCharRect.pivot = new Vector2(.5f, 0);
        rightCharRect.Translate(50f, 0, 0);
        rightCharRect.localScale = new Vector3(-0.9f, .9f, .9f);
        char2.characterText.GetComponent<RectTransform>().localScale = new Vector3(-0.9f, .9f, .9f);
        char2.characterButtonPrompt.localScale = new Vector3(-1, 1, 1);
        char2.characterButtonPrompt.Translate(-310f, 0, 0);

        characterButtonPromptSize[0] = char1.characterButtonPrompt.localScale;
        characterButtonPromptSize[1] = char2.characterButtonPrompt.localScale;
        //char2.characterDialogueBox.localScale = new Vector3(-1, 1, 1);
    }

    public static void ShowDialogue(DialogueSO dialogueSO)
    {
        if (instance.currentDialogueIndex != 0) return;

        instance.canvasGroup.alpha = 0;
        DOTween.To(() => instance.canvasGroup.alpha, x => instance.canvasGroup.alpha = x, 1, 0.25f);

        var children = instance.dialogueParent.GetComponentsInChildren<RectTransform>();
        //instance.uiAnimator.StartAnimation(children, 0.15f, 0.05f, Ease.OutCirc);

        instance.currentDialogueSO = dialogueSO;
        instance.currentDialogueIndex = 0;
        instance.dialogueParent.SetActive(true);

        instance.SetUpCurrentDialogue();
    }

    public void SetUpCurrentDialogue()
    {
        instance.currentCharacterReference = currentDialogueSO.dialogue[currentDialogueIndex].leftSide ? 0 : 1;
        instance.otherCharacterReference = currentDialogueSO.dialogue[currentDialogueIndex].leftSide ? 1 : 0;

        //instance.characters[0].gameObject.SetActive(currentDialogueSO.dialogue[currentDialogueIndex].leftSide);
        //instance.characters[1].gameObject.SetActive(!currentDialogueSO.dialogue[currentDialogueIndex].leftSide);

        instance.characterDialogueBoxes[currentCharacterReference].SetActive(true);
        instance.characterDialogueBoxes[otherCharacterReference].SetActive(false);

        instance.characterImages[currentCharacterReference].DOFade(1f, 0.25f);
        instance.characterImages[otherCharacterReference].DOFade(0.5f, 0.25f);

        instance.characters[instance.currentCharacterReference].gameObject.SetActive(true);
        textToShow = currentDialogueSO.dialogue[currentDialogueIndex].dialogueText;
        instance.characterImages[instance.currentCharacterReference].texture =
            currentDialogueSO.dialogue[currentDialogueIndex].emotion;
        instance.talkSound = currentDialogueSO.dialogue[currentDialogueIndex].character.talkSound;
        TextRoutine = StartCoroutine(ShowTextRoutine());
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    private IEnumerator ShowTextRoutine()
    {
        characterButtonPrompt[currentCharacterReference].DOKill();
        characterButtonPrompt[currentCharacterReference].localScale =
            characterButtonPromptSize[currentCharacterReference];
        finishedWithCurrentText = false;
        var currentText = textToShow;
        characterTexts[instance.currentCharacterReference].text = "";
        for (var i = 0; i < textToShow.Length; i++)
        {
            characterTexts[instance.currentCharacterReference].text += textToShow[i];
            talkSound.Play(audioSource);
            var amount = 0.02f;


            if (i % 3 == 0)
                characterImages[currentCharacterReference].transform
                    .DOPunchScale(
                        new Vector3(
                            Random.Range(0, 1f) * amount,
                            Random.Range(0, 1f) * amount,
                            Random.Range(0, 1f) * amount), 0.2f);

            yield return waitTime;
        }

        finishedWithCurrentText = true;
        //make button prompt scale go up and down
        var side = currentDialogueSO.dialogue[currentDialogueIndex].leftSide ? 1 : -1;
        characterButtonPrompt[currentCharacterReference].DOScale(new Vector3(1.2f * side, 1.2f, 1.2f), 0.2f)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void ForceFinishText()
    {
        StopCoroutine(TextRoutine);
        currentCharacterIndex = currentDialogueSO.dialogue[instance.currentDialogueIndex].dialogueText.Length - 1;
        characterTexts[instance.currentCharacterReference].text = textToShow;
        finishedWithCurrentText = true;

        var side = currentDialogueSO.dialogue[currentDialogueIndex].leftSide ? 1 : -1;
        characterButtonPrompt[currentCharacterReference].DOScale(new Vector3(1.2f * side, 1.2f, 1.2f), 0.2f)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public static void NextDialogue()
    {
        if (instance.finishedWithCurrentText == false)
        {
            instance.ForceFinishText();
        }
        else if (instance.currentDialogueIndex < instance.currentDialogueSO.dialogue.Count - 1)
        {
            instance.finishedWithCurrentText = false;
            instance.currentCharacterIndex = 0;
            instance.StopCoroutine(instance.ShowTextRoutine());
            instance.textToShow = "";
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
        yield return new WaitForSeconds(0.15f);
        instance.onDialogueFinished.Dispatch();
    }

    public static void HideDialogue()
    {
        DOTween.To(() => instance.canvasGroup.alpha, x => instance.canvasGroup.alpha = x, 0, 0.25f).onComplete +=
            () => { instance.dialogueParent.SetActive(false); };
        //instance.canvasGroup.DOFade(0f, 0.25f).onComplete += () => { instance.dialogueParent.SetActive(false); };
    }
}