using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LevelIntroScript : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [SerializeField] private GameObject[] children;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI levelRequisites;
    private LevelRules levelRules;

    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float fadeDelay = 2f;

    public static LevelIntroScript Instance;

    private void Start()
    {
        Instance = this;
        var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        levelText.SetText(sceneName);

        foreach (var child in children) child.SetActive(true);

        levelRequisites.gameObject.SetActive(false);
        StartCoroutine(IntroScene());
    }

    private IEnumerator IntroScene()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        yield return new WaitForSecondsRealtime(fadeDelay);

        DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, fadeDuration).onComplete =
            () => Destroy(gameObject);
    }

    public static void SetLevelRules(LevelRules levelRules)
    {
        Instance.levelRequisites.gameObject.SetActive(true);

        if (levelRules.winByScore)
            Instance.levelRequisites.SetText("Win at least " + LevelManager.ScoreToWin + " points");
    }
}