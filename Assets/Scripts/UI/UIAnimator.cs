using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIAnimator : MonoBehaviour
{
    public void StartAnimation(RectTransform[] children, float duration, float delay, AnimationCurve curve,
        Action onComplete)
    {
        for (var i = 0; i < children.Length; i++)
        {
            children[i].gameObject.SetActive(true);

            if (i < children.Length - 1)
                children[i].GetComponent<RectTransform>().DOScale(0, duration).From().SetDelay(delay * i)
                    .SetEase(curve);
            else
                children[i].GetComponent<RectTransform>().DOScale(0, duration).From().SetDelay(delay * i)
                    .SetEase(curve).OnComplete(() => onComplete?.Invoke());
        }
    }

    public void StartAnimation(RectTransform[] children, float duration, float delay, Ease ease)
    {
        for (var i = 0; i < children.Length; i++)
        {
            children[i].gameObject.SetActive(true);

            children[i].GetComponent<RectTransform>().DOScale(0, duration).From().SetDelay(delay * i)
                .SetEase(ease);
        }
    }

    public void ReverseAnimation(RectTransform[] children, float duration, float delay, Ease ease,
        Action onComplete)
    {
        for (var i = children.Length - 1; i > 0; i--)
            if (i > 1)
                children[i].GetComponent<RectTransform>().DOScale(0, duration)
                    .SetDelay(delay * (children.Length - i))
                    .SetEase(ease);
            else
                children[i].GetComponent<RectTransform>().DOScale(0, duration)
                    .SetDelay(delay * (children.Length - i))
                    .SetEase(ease).OnComplete(() => onComplete?.Invoke());
    }
}