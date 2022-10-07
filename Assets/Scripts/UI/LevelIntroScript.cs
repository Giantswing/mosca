using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LevelIntroScript : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private GameObject transitionImage;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI levelRequisites;

    [SerializeField] private LevelTransitionScript levelTransitionScript;

    private LevelRules levelRules;

    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float fadeDelay = 2f;

    public static LevelIntroScript Instance;

    private void OnEnable()
    {
        Instance = this;
        StartIntroScene();
    }

    public void StartIntroScene()
    {
        var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        levelText.SetText(sceneName);
        levelText.gameObject.SetActive(true);
        levelRequisites.gameObject.SetActive(false);
        transitionImage.SetActive(true);

        StartCoroutine(IntroScene());
    }

    private IEnumerator IntroScene()
    {
        yield return new WaitForSecondsRealtime(fadeDelay);

        DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, fadeDuration).onComplete =
            () =>
            {
                levelTransitionScript.ReverseTransition(Vector3.zero);
                levelText.gameObject.SetActive(false);
                levelRequisites.gameObject.SetActive(false);
            };
    }

    public static void SetLevelRules(LevelRules levelRules)
    {
        Instance.levelRequisites.gameObject.SetActive(true);

        if (levelRules.winByScore)
            Instance.levelRequisites.SetText("Win at least " + LevelManager.ScoreToWin + " points");
    }
}