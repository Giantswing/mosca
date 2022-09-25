using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PortalScript : MonoBehaviour
{
    private void Start()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1.5f, 3f).SetEase(Ease.InOutBounce);
        transform.DORotate(new Vector3(0, 0, 360), 3f, RotateMode.FastBeyond360).SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
    }
}