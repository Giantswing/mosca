using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExitGameSystem : MonoBehaviour
{
    [SerializeField] private InputActionReference exitAction;
    [SerializeField] private CampaignSO campaign;

    private bool canQuit = false;

    public static ExitGameSystem Instance { get; private set; }

    private void Awake()
    {
        Instance = this;


        DOVirtual.DelayedCall(0.5f, () => canQuit = true);
    }


    private void OnEnable()
    {
        exitAction.action.canceled += _ => QuitGame();
        exitAction.action.Enable();
    }

    private void OnDisable()
    {
        exitAction.action.Disable();
    }

    public void QuitGame()
    {
        if (!canQuit) return;
        canQuit = false;

        if (gameObject != null)
            gameObject.SetActive(false);

        print("exiting");

        DOTween.KillAll();
        var transitionType = (int)LevelLoader.LevelTransitionState.SpecificLevel;

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "_levelSelection")
            LevelLoadSystem.LoadSpecificLevel(campaign.mainMenuScene);
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "main-menu")
            Application.Quit();
        else
            LevelLoadSystem.LoadSpecificLevel(campaign.levelSelectionScene);
    }

    public static void QuitGameStatic()
    {
        Instance.QuitGame();
    }
}