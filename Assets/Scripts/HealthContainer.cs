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

    private List<GameObject> _heartContainers = new();
    private int _maxHeartCointainersUI = 10;


    private void Awake()
    {
        PlayerInteractionHandler.OnPlayerHealthChange += PlayerHealthChanged;
    }

    private void Start()
    {
        for (var i = 0; i < _maxHeartCointainersUI; i++)
        {
            _heartContainers.Add(Instantiate(heartContainerPrefab, transform));
            _heartContainers[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(i * 50, 0);
            _heartContainers[i].SetActive(false);
        }
    }


    private void OnDisable()
    {
        PlayerInteractionHandler.OnPlayerHealthChange -= PlayerHealthChanged;
    }

    private void PlayerHealthChanged(int health, int maxHealth)
    {
        for (var i = 0; i < _maxHeartCointainersUI; i++)
            if (i < maxHealth)
            {
                _heartContainers[i].SetActive(true);
                if (i < health)
                    _heartContainers[i].GetComponent<Image>().sprite = fullHeartContainerSprite;
                else
                    _heartContainers[i].GetComponent<Image>().sprite = emptyHeartContainerSprite;

                if (i == health)
                    _heartContainers[i].transform.DOPunchScale(Vector3.one * .4f, .2f);
            }
            else
            {
                _heartContainers[i].SetActive(false);
            }
    }
}