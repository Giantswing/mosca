using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PortalScript : MonoBehaviour
{
    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1.5f, 1f).SetEase(Ease.InQuart);
        transform.DORotate(new Vector3(0, 0, 360), 2f, RotateMode.FastBeyond360).SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
    }
}