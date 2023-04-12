using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PortalPopUpScript : MonoBehaviour
{
    [SerializeField] private RectTransform popUpRect;
    [SerializeField] private RectTransform popUpBG;
    [SerializeField] private TextMeshProUGUI popUpText;
    [SerializeField] private RawImage popUpIcon;
    private bool isIcon = false;
    private bool isHidden = false;
    [HideInInspector] public Transform portalTransform;
    private Vector3 _auxPos;
    private Camera _mainCamera;
    [SerializeField] private float padding;

    [SerializeField] private SimpleAudioEvent notificationAudioEvent;
    private float scaleTo;


    private void OnEnable()
    {
        _mainCamera = Camera.main;
        isIcon = false;
        scaleTo = .75F;
        popUpRect.localScale *= scaleTo;
        popUpRect.DOScale(0, 0.5f).From();
        StartCoroutine(ChangeState());
        SoundMaster.PlaySound(transform.position, (int)SoundListAuto.UINotification, false);
    }

    private IEnumerator ChangeState()
    {
        yield return new WaitForSecondsRealtime(2f);

        popUpText.DOFade(0, 0.1f);
        popUpBG.DOSizeDelta(new Vector2(0, 0), 0.3f).onComplete += () => { Destroy(gameObject); };
        /*
        popUpBG.DOSizeDelta(new Vector2(50f, 50f), 0.3f).onComplete += () =>
        {
            isIcon = true;
            popUpIcon.DOFade(0f, 0.2f).onComplete += () => { Destroy(gameObject); };
        };
        */
    }

    private void Update()
    {
        if (!isIcon) return;

        _auxPos = portalTransform.position;
        _auxPos = _mainCamera.WorldToScreenPoint(_auxPos);
        //check if auxPos is inside the screen
        if (_auxPos.x < 0 || _auxPos.x > Screen.width || _auxPos.y < 0 || _auxPos.y > Screen.height)
        {
            if (!isHidden)
            {
                isHidden = true;
                popUpRect.DOScale(scaleTo, 0.1f);
            }
        }
        else
        {
            if (isHidden)
            {
                isHidden = false;
                popUpRect.DOScale(0, 0.1f);
            }
        }

        _auxPos = new Vector3(Mathf.Clamp(_auxPos.x, padding, Screen.width - padding),
            Mathf.Clamp(_auxPos.y, padding, Screen.height - padding),
            _auxPos.z);

        popUpRect.position = Vector3.Lerp(popUpRect.position, _auxPos, 0.06f);
    }
}