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

    [SerializeField] private SmartData.SmartInt.IntWriter playerHealth;
    [SerializeField] private SmartData.SmartInt.IntWriter playerMaxHealth;

    private List<GameObject> _heartContainers = new();
    private int _maxHeartCointainersUI = 5;


    private void Start()
    {
        for (var i = 0; i < _maxHeartCointainersUI; i++)
        {
            _heartContainers.Add(Instantiate(heartContainerPrefab, transform));
            _heartContainers[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(i * 50, 0);
            _heartContainers[i].SetActive(false);
        }

        PlayerHealthChange();
    }


    public void PlayerHealthChange()
    {
        if (playerHealth.value > playerMaxHealth.value)
            playerHealth.value = playerMaxHealth.value;


        for (var i = 0; i < _maxHeartCointainersUI; i++)
            if (i < playerMaxHealth)
            {
                _heartContainers[i].SetActive(true);
                if (i < playerHealth)
                    _heartContainers[i].GetComponent<Image>().sprite = fullHeartContainerSprite;
                else
                    _heartContainers[i].GetComponent<Image>().sprite = emptyHeartContainerSprite;

                if (i == playerHealth)
                    _heartContainers[i].transform.DOPunchScale(Vector3.one * .4f, .2f);
            }
            else
            {
                _heartContainers[i].SetActive(false);
            }
    }
}