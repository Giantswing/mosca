using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject pauseWindow;
    public GameObject coopWindow;
    public bool isPaused = false;
    public TextMeshProUGUI coopPlayersText;

    private bool isAnimating = false;
    public GameObject firstSelected;
    public GameObject coopFirstSelected;
    public static PauseMenuScript instance;


    [SerializeField] private InputActionReference pauseAction;

    private void Awake()
    {
        instance = this;
        isPaused = false;
        isAnimating = false;
        pauseMenu.SetActive(false);
    }

    private void OnEnable()
    {
        pauseAction.action.canceled += _ => TogglePause();
        pauseAction.action.Enable();
    }

    private void OnDisable()
    {
        pauseAction.action.Disable();
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public static void UpdatePlayerText()
    {
        var playerText = "";

        for (var i = 0; i < TargetGroupControllerSystem.Instance._playerInputManager.playerCount; i++)
        {
            //use string interpolation
            PlayerInput playerInput = TargetGroupControllerSystem.Instance.playerList[i].playerInput;
            string textToAdd = playerInput.devices[0].name;
            //replace DualSenseGamepadHID with PS5 Gamepad

            textToAdd = textToAdd.Replace("DualSenseGamepadHID", "PS5 Gamepad ");
            textToAdd = textToAdd.Replace("XInputControllerWindows", "Generic Gamepad ");

            playerText += $"Player {i + 1}: {textToAdd}\n";
        }

        instance.coopPlayersText.SetText(playerText);
    }

    public static void PauseGame()
    {
        if (instance.isPaused || instance.isAnimating) return;
        TargetGroupControllerSystem.ChangePlayersEnabled(false);

        instance.pauseMenu.SetActive(true);
        instance.pauseWindow.SetActive(true);
        instance.coopWindow.SetActive(false);

        Time.timeScale = 0;
        instance.isPaused = true;
        instance.isAnimating = true;

        instance.pauseWindow.transform.localScale = Vector3.zero;
        EventSystemScript.ChangeFirstSelected(instance.firstSelected);

        instance.pauseWindow.transform.DOScale(0.5f, .65f).SetEase(Ease.OutBack).SetUpdate(true)
            .OnComplete(() => { instance.isAnimating = false; });
    }

    public static void ResumeGame()
    {
        if (!instance.isPaused || instance.isAnimating) return;

        Time.timeScale = 1;
        instance.isPaused = false;

        instance.pauseWindow.SetActive(true);
        instance.coopWindow.SetActive(false);

        TargetGroupControllerSystem.DisableJoining();

        instance.pauseWindow.transform.DOScale(0, 0.5f).SetEase(Ease.InBack).SetUpdate(true)
            .OnComplete(() =>
            {
                instance.isAnimating = false;
                instance.pauseMenu.SetActive(false);

                DOVirtual.DelayedCall(.3f, () => { TargetGroupControllerSystem.ChangePlayersEnabled(true); })
                    .SetUpdate(true);
            });
    }

    public void CoopStartWindow()
    {
        TargetGroupControllerSystem.AllowCoop();
        instance.pauseWindow.SetActive(false);
        instance.coopWindow.SetActive(true);
        EventSystemScript.ChangeFirstSelected(instance.coopFirstSelected);
        UpdatePlayerText();
    }


    public void CoopExitWindow()
    {
        TargetGroupControllerSystem.DisallowCoop();
        TargetGroupControllerSystem.DisableJoining();
        instance.pauseWindow.SetActive(true);
        instance.coopWindow.SetActive(false);
        EventSystemScript.ChangeFirstSelected(instance.firstSelected);
    }

    public void Resume()
    {
        ResumeGame();
    }

    public void Exit()
    {
        Time.timeScale = 1;
        LevelLoadSystem.LoadLevel(LevelLoadSystem.LevelToLoad.LevelSelection);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        LevelLoadSystem.LoadLevel(LevelLoadSystem.LevelToLoad.Restart);
    }
}