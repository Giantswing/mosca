using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveLoadSystem : MonoBehaviour
{
    [SerializeField] private CampaignSO campaign;
    public static SaveLoadSystem Instance { get; private set; }

    [SerializeField] private InputAction quitAction;

    private void Awake()
    {
        Instance = this;

        quitAction.performed += _ => QuitGame();
        quitAction.Enable();
    }

    private void OnDisable()
    {
        quitAction.Disable();
    }

    public static void SaveGame()
    {
        var levelStars = new int[Instance.campaign.levels.Count];

        for (var i = 0; i < Instance.campaign.levels.Count; i++) levelStars[i] = Instance.campaign.levels[i].stars;

        PlayerPrefsX.SetIntArray("levelStars", levelStars);
    }

    public static void LoadGame()
    {
        if (PlayerPrefsX.GetIntArray("levelStars").Length == 0) return;

        for (var i = 0; i < PlayerPrefsX.GetIntArray("levelStars").Length; i++)
            Instance.campaign.levels[i].stars = PlayerPrefsX.GetIntArray("levelStars")[i];
    }

    private void QuitGame()
    {
        print("bye bye");
        Application.Quit();
    }
}