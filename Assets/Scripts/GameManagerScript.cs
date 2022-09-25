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

    private float _updateFPS = .1f;
    [SerializeField] private TextMeshProUGUI fpsText;

    // Level and level transition management
    public static UnityAction StartLevelTransition;

    private void Start()
    {
        //singleton code
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(transform.root.gameObject);
    }

    private void Update()
    {
        if (Instance._updateFPS >= 0)
        {
            Instance._updateFPS -= Time.deltaTime;
            if (Instance._updateFPS < 0)
            {
                fpsText.SetText(Mathf.Round(1f / Time.unscaledDeltaTime).ToString());
                Instance._updateFPS = .1f;
            }
        }
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