using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MinimapScript : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RawImage meshRenderer;

    private Vector2 _minimapOriginalPos;
    private Vector2 _anchorCenter;
    private Vector2 _anchorTop;
    private float _minimapOriginalSize;

    private Vector2 _minimapPositionTo;
    private Vector2 _minimapAnchorTo;
    private float _minimapOpacityTo;
    private float _minimapSizeTo;

    private static readonly int Opacity = Shader.PropertyToID("_Opacity");

    private void Awake()
    {
        _anchorCenter = new Vector2(0.5f, 0.5f);
        _anchorTop = new Vector2(0.5f, 1f);

        _minimapOriginalPos = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
        _minimapOriginalSize = rectTransform.sizeDelta.x;
    }

    private void OnEnable()
    {
        PlayerCamera.OnMapToggle += MapToggle;
    }

    private void OnDisable()
    {
        PlayerCamera.OnMapToggle -= MapToggle;
    }

    private void MapToggle(bool state)
    {
        if (state)
        {
            //if map is enlarged
            _minimapPositionTo = Vector2.zero;
            _minimapAnchorTo = _anchorCenter;
            _minimapSizeTo = _minimapOriginalSize * 2f;
            _minimapOpacityTo = .9f;
        }
        else
        {
            //if map is enlarged
            _minimapPositionTo = _minimapOriginalPos;
            _minimapAnchorTo = _anchorTop;
            _minimapSizeTo = _minimapOriginalSize;
            _minimapOpacityTo = .5f;
        }

        DOTween.To(() => rectTransform.anchoredPosition, x => rectTransform.anchoredPosition = x, _minimapPositionTo,
            .5f);

        DOTween.To(() => rectTransform.anchorMax, x => rectTransform.anchorMax = x, _minimapAnchorTo, .5f);
        DOTween.To(() => rectTransform.anchorMin, x => rectTransform.anchorMin = x, _minimapAnchorTo, .5f);


        DOTween.To(() => rectTransform.sizeDelta, x => rectTransform.sizeDelta = x,
            new Vector2(_minimapSizeTo, _minimapSizeTo), .5f);

        DOTween.To(() => meshRenderer.material.GetFloat(Opacity), x => meshRenderer.material.SetFloat(Opacity, x),
            _minimapOpacityTo, .5f);

        //tween pivot too
        DOTween.To(() => rectTransform.pivot, x => rectTransform.pivot = x, _minimapAnchorTo, .5f);

/*
        //rectTransform.anchoredPosition = _minimapPositionTo;
        rectTransform.anchorMin = _minimapAnchorTo;
        rectTransform.anchorMax = _minimapAnchorTo;
        rectTransform.pivot = _minimapAnchorTo;
        rectTransform.sizeDelta = new Vector2(_minimapSizeTo, _minimapSizeTo);
        meshRenderer.material.SetFloat(Opacity, _minimapOpacityTo);
        */
    }
}