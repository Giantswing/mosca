using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript Instance;

    // Level and level transition management
    public static UnityAction StartLevelTransition;

    private void Start()
    {
        //singleton code
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        //Application.targetFrameRate = 25;
        DontDestroyOnLoad(transform.root.gameObject);
    }


    public static void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        PlayerInteractionHandler.OnScoreChanged?.Invoke(0);
    }

    public static void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        PlayerInteractionHandler.OnScoreChanged?.Invoke(0);
    }
}