using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HeartContainersUI : MonoBehaviour
{
    [SerializeField] private CampaignSO campaignSO;
    [SerializeField] private Texture[] heartContainersTexture;

    private RawImage _heartImage;
    private Vector3 _heartScale;
    private bool canDoAnimation = false;

    public static Action OnHeartFilledAnimationEnd;

    private void OnEnable()
    {
        LevelManager.OnHeartContainersChanged += UpdateHeartContainersUI;
    }

    private void OnDisable()
    {
        LevelManager.OnHeartContainersChanged -= UpdateHeartContainersUI;
    }

    private void Awake()
    {
        _heartImage = GetComponent<RawImage>();
        _heartScale = _heartImage.transform.localScale;
    }

    private void Start()
    {
        campaignSO = LevelManager.GetCurrentCampaign();
        UpdateHeartContainersUI();

        DOVirtual.DelayedCall(0.5f, () => canDoAnimation = true);
    }

    public void UpdateHeartContainersUI()
    {
        var heartContainers = campaignSO.heartContainers % 3;

        if (heartContainers != 0 || !canDoAnimation)
        {
            _heartImage.texture = heartContainersTexture[heartContainers];
            _heartImage.transform.DOPunchScale(_heartScale * 0.4f, 0.25f, 1, 0.5f).onComplete += () =>
            {
                _heartImage.transform.localScale = _heartScale;
            };

            _heartImage.transform.DOPunchRotation(new Vector3(0, 0, 10), 0.25f, 1, 0.5f);
        }
        else
        {
            FillContainerAnimation();
        }
    }

    private void FillContainerAnimation()
    {
        _heartImage.texture = heartContainersTexture[3];
        var heartContainers = campaignSO.heartContainers % 3;

        _heartImage.transform.DOShakeRotation(1.2f, 20, 20, 90, false);
        _heartImage.transform.DOScale(_heartScale * 2f, 1.5f).onComplete += () =>
        {
            _heartImage.texture = heartContainersTexture[heartContainers];
            _heartImage.transform.localScale = Vector3.one * 0.25f;
            _heartImage.transform.DOScale(_heartScale, 0.8f).SetEase(Ease.OutElastic);
            OnHeartFilledAnimationEnd?.Invoke();
        };
    }
}