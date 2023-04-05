using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class ChargeUI : MonoBehaviour
{
    [SerializeField] private Transform chargeCircle;
    [SerializeField] private Transform chargeArrow;
    [SerializeField] private LineRenderer line;
    [SerializeField] private PlayerMovement _playerMovement;

    [SerializeField] private ChargeShot chargeShot;
    private Vector3 pointDirection;
    private float strength;
    private bool isActive = false;

    private void Start()
    {
        chargeCircle.gameObject.SetActive(false);
        chargeArrow.gameObject.SetActive(false);
        line.enabled = false;
    }

    private void OnEnable()
    {
        chargeShot.Charging += UpdateUI;
        chargeShot.Release += Release;
    }

    private void OnDisable()
    {
        chargeShot.Charging -= UpdateUI;
        chargeShot.Release -= Release;
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
                _playerMovement.transform.position + pointDirection.normalized * strength * 2.3f;
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

    private void UpdateUI(Vector3 dir, float strength)
    {
        pointDirection = dir;
        this.strength = strength;
    }

    private void Release()
    {
        DOTween.To(() => strength, x => strength = x, 0, 0.45f).SetEase(Ease.InElastic).onComplete += () =>
        {
            chargeCircle.gameObject.SetActive(false);
            chargeArrow.gameObject.SetActive(false);
        };
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, _playerMovement.transform.right);
    }
}