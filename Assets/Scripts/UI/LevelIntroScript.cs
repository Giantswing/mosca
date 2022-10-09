using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LevelIntroScript : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private GameObject transitionImage;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI levelObjectivesText;


    [SerializeField] private LevelTransitionScript levelTransitionScript;

    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float fadeDelay = 2f;

    private void Start()
    {
        StartIntroScene();
    }

    public void StartIntroScene()
    {
        if (levelNameText != null)
        {
            levelNameText.SetText(LevelManager.LevelData().sceneName);
            levelObjectivesText.SetText(
                "Win <color=red>at least</color> " + LevelManager.LevelData().scoreToWin +
                " points\nBonus time: " + LevelManager.LevelData().timeToWin + " seconds");
            levelNameText.gameObject.SetActive(true);
            levelObjectivesText.gameObject.SetActive(true);
            transitionImage.SetActive(true);

            StartCoroutine(IntroScene());
        }
        else
        {
            transitionImage.SetActive(true);
            levelTransitionScript.ReverseTransition(Vector3.zero);
        }
    }

    private IEnumerator IntroScene()
    {
        yield return new WaitForSecondsRealtime(fadeDelay);

        DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, fadeDuration).onComplete =
            () =>
            {
                levelTransitionScript.ReverseTransition(Vector3.zero);
                levelNameText.gameObject.SetActive(false);
                levelObjectivesText.gameObject.SetActive(false);
            };
    }
}