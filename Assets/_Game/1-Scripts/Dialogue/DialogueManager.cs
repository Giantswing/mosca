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

    [SerializeField] private Material Mat1;
    [SerializeField] private Material Mat2;

    public Coroutine TextRoutine = null;
    private static readonly int MainTex = Shader.PropertyToID("_CharTexture");
    private static readonly int OpacityMult = Shader.PropertyToID("_OpacityMult");

    public static bool isInverted = false;


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
        DialogueReferences char1, char2;

        char1 = Instantiate(dialoguePrefab, dialogueParent.transform).GetComponent<DialogueReferences>();
        char2 = Instantiate(dialoguePrefab, dialogueParent.transform).GetComponent<DialogueReferences>();


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


        characterImages[0].material = Mat1;
        characterImages[1].material = Mat2;


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

    private void StartDefaultCharacters()
    {
        var foundChar1Image = false;
        var foundChar2Image = false;

        for (var i = 0; i < instance.currentDialogueSO.dialogueList.Count; i++)
        {
            if (instance.currentDialogueSO.dialogueList[i].leftSide && foundChar1Image == false)
            {
                instance.characterImages[0].material
                    .SetTexture(MainTex,
                        instance.currentDialogueSO.dialogueList[i].character.startingEmotion.sprite);
                foundChar1Image = true;
            }

            if (!instance.currentDialogueSO.dialogueList[i].leftSide && foundChar2Image == false)
            {
                instance.characterImages[1].material
                    .SetTexture(MainTex,
                        instance.currentDialogueSO.dialogueList[i].character.startingEmotion.sprite);
                foundChar2Image = true;
            }
        }
    }

    private void StartDefaultCharactersFromDialogue()
    {
        instance.characterImages[isInverted ? 1 : 0].material
            .SetTexture(MainTex,
                instance.currentDialogueSO.startingLeftEmotion);

        instance.characterImages[isInverted ? 0 : 1].material
            .SetTexture(MainTex,
                instance.currentDialogueSO.startingRightEmotion);
    }

    public static void ShowDialogue(DialogueSO dialogueSO)
    {
        if (instance.currentDialogueIndex != 0) return;


        instance.canvasGroup.alpha = 0;
        DOTween.To(() => instance.canvasGroup.alpha, x => instance.canvasGroup.alpha = x, 1, 0.5f);


        for (var i = 0; i < 2; i++)
        {
            instance.characterImages[i].material.SetFloat(OpacityMult, 0);
            instance.characterImages[i].material.DOFloat(1f, OpacityMult, 0.25f);
        }

        var children = instance.dialogueParent.GetComponentsInChildren<RectTransform>();
        //instance.uiAnimator.StartAnimation(children, 0.15f, 0.05f, Ease.OutCirc);


        instance.currentDialogueSO = dialogueSO;
        instance.currentDialogueIndex = 0;
        instance.dialogueParent.SetActive(true);

        var foundChar1Image = false;
        var foundChar2Image = false;

        for (var i = 0; i < instance.currentDialogueSO.dialogueList.Count; i++)
        {
            if (instance.currentDialogueSO.dialogueList[i].leftSide && foundChar1Image == false)
            {
                instance.characterImages[0].material
                    .SetTexture(MainTex,
                        instance.currentDialogueSO.dialogueList[i].character
                            .emotions[instance.currentDialogueSO.dialogueList[i].emotionIndex].sprite);
                foundChar1Image = true;
            }

            if (!instance.currentDialogueSO.dialogueList[i].leftSide && foundChar2Image == false)
            {
                instance.characterImages[1].material
                    .SetTexture(MainTex,
                        instance.currentDialogueSO.dialogueList[i].character
                            .emotions[instance.currentDialogueSO.dialogueList[i].emotionIndex].sprite);
                foundChar2Image = true;
            }
        }

        //instance.StartDefaultCharacters();
        instance.StartDefaultCharactersFromDialogue();
        instance.SetUpCurrentDialogue();
    }

    public void SetUpCurrentDialogue()
    {
        if (!isInverted)
        {
            instance.currentCharacterReference = currentDialogueSO.dialogueList[currentDialogueIndex].leftSide ? 0 : 1;
            instance.otherCharacterReference = currentDialogueSO.dialogueList[currentDialogueIndex].leftSide ? 1 : 0;
        }
        else
        {
            instance.currentCharacterReference = currentDialogueSO.dialogueList[currentDialogueIndex].leftSide ? 1 : 0;
            instance.otherCharacterReference = currentDialogueSO.dialogueList[currentDialogueIndex].leftSide ? 0 : 1;
        }


        //instance.characters[0].gameObject.SetActive(currentDialogueSO.dialogue[currentDialogueIndex].leftSide);
        //instance.characters[1].gameObject.SetActive(!currentDialogueSO.dialogue[currentDialogueIndex].leftSide);

        instance.characterDialogueBoxes[currentCharacterReference].SetActive(true);
        instance.characterDialogueBoxes[otherCharacterReference].SetActive(false);


        instance.characterImages[currentCharacterReference].material.DOFloat(1f, "_Saturation", 0.25f);
        instance.characterImages[currentCharacterReference].material.DOFloat(1f, "_Brightness", 0.25f);

        instance.characterImages[otherCharacterReference].material.DOFloat(0f, "_Saturation", 0.25f);
        instance.characterImages[otherCharacterReference].material.DOFloat(0.2f, "_Brightness", 0.25f);

        instance.characters[instance.currentCharacterReference].gameObject.SetActive(true);
        textToShow = currentDialogueSO.dialogueList[currentDialogueIndex].dialogueText;


        instance.characterImages[instance.currentCharacterReference].material
            .SetTexture(MainTex,
                instance.currentDialogueSO.dialogueList[currentDialogueIndex].character
                    .emotions[instance.currentDialogueSO.dialogueList[currentDialogueIndex].emotionIndex].sprite);
        /*
        instance.characterImages[instance.currentCharacterReference].material
            .SetTexture(MainTex, currentDialogueSO.dialogueList[currentDialogueIndex].emotion);
            */


        instance.talkSound = currentDialogueSO.dialogueList[currentDialogueIndex].character.talkSound;
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

            if (i % 2 == 0)
                talkSound.Play(audioSource);

            var amount = 0.025f;
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

        int side;
        if (!isInverted)
            side = currentDialogueSO.dialogueList[currentDialogueIndex].leftSide ? 1 : -1;
        else
            side = currentDialogueSO.dialogueList[currentDialogueIndex].leftSide ? -1 : 1;

        characterButtonPrompt[currentCharacterReference].DOScale(new Vector3(1.2f * side, 1.2f, 1.2f), 0.2f)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void ForceFinishText()
    {
        StopCoroutine(TextRoutine);
        currentCharacterIndex = currentDialogueSO.dialogueList[instance.currentDialogueIndex].dialogueText.Length - 1;
        characterTexts[instance.currentCharacterReference].text = textToShow;
        finishedWithCurrentText = true;

        int side;
        if (!isInverted)
            side = currentDialogueSO.dialogueList[currentDialogueIndex].leftSide ? 1 : -1;
        else
            side = currentDialogueSO.dialogueList[currentDialogueIndex].leftSide ? -1 : 1;

        characterButtonPrompt[currentCharacterReference].DOScale(new Vector3(1.2f * side, 1.2f, 1.2f), 0.2f)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public static void NextDialogue()
    {
        if (instance.finishedWithCurrentText == false)
        {
            instance.ForceFinishText();
        }
        else if (instance.currentDialogueIndex < instance.currentDialogueSO.dialogueList.Count - 1)
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
        for (var i = 0; i < 2; i++)
        {
            instance.characterImages[i].material.SetFloat(OpacityMult, 1);
            instance.characterImages[i].material.DOFloat(0f, OpacityMult, 0.25f);
        }

        DOTween.To(() => instance.canvasGroup.alpha, x => instance.canvasGroup.alpha = x, 0, 0.5f).onComplete +=
            () => { instance.dialogueParent.SetActive(false); };
        //instance.canvasGroup.DOFade(0f, 0.25f).onComplete += () => { instance.dialogueParent.SetActive(false); };
    }
}