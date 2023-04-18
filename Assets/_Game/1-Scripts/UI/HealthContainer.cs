using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthContainer : MonoBehaviour
{
    [SerializeField] private Sprite fullHeartContainerSprite;
    [SerializeField] private Sprite emptyHeartContainerSprite;


    [SerializeField] private GameObject heartContainerPrefab;
    [SerializeField] private AttributeDataSO playerData;

    private CanvasGroup _canvasGroup;

    private List<GameObject> _heartContainers = new();
    private int _maxHeartCointainersUI = 20;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        HeartContainersUI.OnHeartFilledAnimationEnd += InitializeHealth;
    }

    private void OnDisable()
    {
        HeartContainersUI.OnHeartFilledAnimationEnd -= InitializeHealth;
        playerData.attributes.onHeal.RemoveListener(PlayerHealthUpdate);
        playerData.attributes.onReceiveHit.RemoveListener(PlayerHealthUpdate);
    }

    private void Start()
    {
        playerData.attributes.onHeal.AddListener(PlayerHealthUpdate);
        playerData.attributes.onReceiveHit.AddListener(PlayerHealthUpdate);


        DOVirtual.DelayedCall(0.1f, () => { InitializeHealth(); });
    }

    public void InitializeHealth()
    {
        //delete previous heart containers if there are any

        foreach (GameObject heartContainer in _heartContainers)
            Destroy(heartContainer);

        _heartContainers.Clear();
        for (var i = 0; i < _maxHeartCointainersUI; i++)
        {
            _heartContainers.Add(Instantiate(heartContainerPrefab, transform));
            _heartContainers[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(i * 50, 0);
            _heartContainers[i].SetActive(false);
        }

        //PlayerHealthChange();
        PlayerHealthUpdate();
    }


    public void PlayerHealthUpdate()
    {
        if (playerData.attributes.HP > 0)
        {
            _canvasGroup.alpha = 1;

            for (var i = 0; i < _maxHeartCointainersUI; i++)
                if (i < playerData.attributes.maxHP)
                {
                    _heartContainers[i].SetActive(true);
                    if (i < playerData.attributes.HP)
                        _heartContainers[i].GetComponent<Image>().sprite = fullHeartContainerSprite;
                    else
                        _heartContainers[i].GetComponent<Image>().sprite = emptyHeartContainerSprite;

                    if (i == playerData.attributes.HP)
                        _heartContainers[i - 1].transform.DOPunchScale(Vector3.one * .6f, .2f);
                }
                else
                {
                    _heartContainers[i].SetActive(false);
                }
        }
        else
        {
            _canvasGroup.alpha = 0;
        }
    }
}