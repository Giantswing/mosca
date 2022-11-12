using System;
using _Game._1_Scripts.UI;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TutorialBehaviour : MonoBehaviour
{
    [SerializeField] private TutorialSO tutorialData;
    [SerializeField] private GIFRenderer gifRenderer;
    [SerializeField] private TextMeshProUGUI tutorialTitle;
    [SerializeField] private TextMeshProUGUI tutorialDescription;
    [SerializeField] private GameObject tutorialButton;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private SmartData.SmartBool.BoolReader showIntro;

    private void Awake()
    {
        if (tutorialData == null || !showIntro) Destroy(gameObject);

        tutorialPanel.SetActive(true);

        gifRenderer.frames = tutorialData.tutorialImages;
        transform.localScale = Vector3.zero;

        tutorialTitle.text = tutorialData.tutorialName;

        if (Application.platform == RuntimePlatform.Android)
            tutorialDescription.text = tutorialData.tutorialAndroidTexts[0];
        else
            tutorialDescription.text = tutorialData.tutorialTexts[0];
    }

    public void StartTutorial()
    {
        Time.timeScale = 0;
        print("Start Tutorial");
        transform.DOScale(1, 0.35f).SetUpdate(true);
        EventSystemScript.ChangeFirstSelected(tutorialButton);
    }

    public void Proceed()
    {
        transform.DOScale(0, 0.15f).SetUpdate(true).onComplete += () =>
        {
            Time.timeScale = 1;
            Destroy(gameObject);
        };
    }
}