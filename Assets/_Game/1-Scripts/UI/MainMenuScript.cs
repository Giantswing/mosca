using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI startGameText;
    [SerializeField] private RectTransform mainLogo;
    [SerializeField] private RectTransform menuButtons;

    [Space(10)] [SerializeField] private GameObject backgroundFliesObject;
    private List<RectTransform> _backgroundFlies;


    /*-----------------------------*/

    [Space(25)] [SerializeField] private InputAction startGameAction;

    /*-----------------------------*/

    [Space(25)] [Header("Data")] [SerializeField]
    private SmartData.SmartInt.IntWriter transitionType;

    [SerializeField] private SmartData.SmartEvent.EventDispatcher transitionEvent;
    [SerializeField] private SmartData.SmartBool.BoolWriter showIntro;
    [SerializeField] private SmartData.SmartBool.BoolWriter showIntroText;
    private bool _isTransitioning = false;


    private void Awake()
    {
        startGameAction.performed += _ => StartMenu();
        startGameAction.Enable();

        if (Application.platform == RuntimePlatform.Android)
            startGameText.text = "Tap to start";
        else
            startGameText.text = "Press any button or key to start";

        startGameText.DOFade(0.25f, 0.65f).SetLoops(-1, LoopType.Yoyo);

        _backgroundFlies = new List<RectTransform>();
        foreach (Transform child in backgroundFliesObject.transform)
            _backgroundFlies.Add(child.GetComponent<RectTransform>());

        foreach (var fly in _backgroundFlies)
        {
            fly.DOShakePosition(1.5f, 4f, 10, 90, false, true).SetLoops(-1, LoopType.Yoyo);
            fly.DOShakeRotation(1.5f, 10, 10, 90, false).SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void OnDisable()
    {
        startGameAction.Disable();
    }

    private void StartMenu()
    {
        startGameAction.Disable();
        print("Game Started");
        startGameText.DOKill();
        startGameText.DOFade(0, 0.5f);

        mainLogo.DOMove(new Vector3(Screen.width * 0.3f, Screen.height * 0.55f, 0), 0.5f);
        menuButtons.gameObject.SetActive(true);
        menuButtons.DOMoveX(Screen.width * 0.7f, 0.5f);

        EventSystemScript.ChangeFirstSelected(menuButtons.gameObject.GetComponentsInChildren<RectTransform>()[1]
            .gameObject);
    }


    public void StartGame()
    {
        if (!_isTransitioning)
        {
            showIntro.value = true;
            showIntroText.value = true;
            transitionType.value = (int)LevelLoader.LevelTransitionState.LoadNextInBuild;
            transitionEvent.Dispatch();
            _isTransitioning = true;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}