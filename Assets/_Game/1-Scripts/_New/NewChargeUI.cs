using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class NewChargeUI : MonoBehaviour
{
    [SerializeField] private Transform chargeCircle;
    [SerializeField] private Transform chargeArrow;
    [SerializeField] private LineRenderer line;

    private Vector3 pointDirection;
    private float strength;
    private bool isActive = false;

    private void Awake()
    {
        chargeCircle.gameObject.SetActive(false);
        chargeArrow.gameObject.SetActive(false);
        line.enabled = false;
    }

    private void LateUpdate()
    {
        if (strength > 0)
        {
            if (!isActive)
            {
                chargeCircle.gameObject.SetActive(true);
                chargeArrow.gameObject.SetActive(true);
                isActive = true;
            }

            chargeCircle.transform.localScale = Vector3.one * strength * 2f;


            chargeArrow.transform.position =
                transform.position + pointDirection.normalized * strength * 2.3f;
            chargeArrow.transform.rotation = Quaternion.LookRotation(pointDirection, Vector3.up);
        }
        else
        {
            if (isActive)
            {
                chargeCircle.gameObject.SetActive(false);
                chargeArrow.gameObject.SetActive(false);
                isActive = false;
            }
        }
    }

    public void UpdateUI(Vector3 dir, float strength)
    {
        pointDirection = dir;
        this.strength = strength;
    }

    public void Release()
    {
        DOTween.To(() => strength, x => strength = x, 0, 0.45f).SetEase(Ease.InElastic).onComplete += () =>
        {
            chargeCircle.gameObject.SetActive(false);
            chargeArrow.gameObject.SetActive(false);
        };
    }
}