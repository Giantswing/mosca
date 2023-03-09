using System;
using DG.Tweening;
using UnityEngine;

public class ElevatorScriptDoor : MonoBehaviour
{
    public bool isDoorOpen;
    [SerializeField] private float doorHeight = 2.9f;

    public Ease easeOpen = Ease.InOutQuad;
    public Ease easeClose = Ease.InOutQuad;

    private ParticleSystem closingFx;

    [SerializeField] private SimpleAudioEvent doorClosingSound;
    [SerializeField] private SimpleAudioEvent doorHitSound;

    private void Start()
    {
        easeOpen = Ease.InCirc;
        easeClose = Ease.OutBounce;
        closingFx = GetComponent<ParticleSystem>();
    }

    public void OpenDoor()
    {
        if (isDoorOpen == false)
        {
            doorClosingSound.pitch.minValue = 1.2f;
            doorClosingSound.pitch.maxValue = 1.4f;
            GlobalAudioManager.PlaySound(doorClosingSound, transform.position);
            isDoorOpen = true;
            transform.DOLocalMoveY(0, 1f).SetEase(easeOpen);
        }
    }


    public void CloseDoor()
    {
        if (isDoorOpen)
        {
            isDoorOpen = false;
            transform.DOLocalMoveY(-doorHeight, 1f).SetEase(easeClose).SetDelay(0.2f);
        }
    }

    public void CloseDoor(Action callback)
    {
        if (isDoorOpen)
        {
            doorClosingSound.pitch.minValue = 0.9f;
            doorClosingSound.pitch.maxValue = 1.1f;
            GlobalAudioManager.PlaySound(doorClosingSound, transform.position);
            isDoorOpen = false;
            transform.DOLocalMoveY(-doorHeight, 1f).SetEase(easeClose).SetDelay(0.25f).OnComplete(() =>
            {
                callback();
            });
            transform.DOLocalMoveX(transform.localPosition.x, 0.6f).onComplete += () =>
            {
                closingFx.Emit(20);
                GlobalAudioManager.PlaySound(doorHitSound, transform.position);
            };
        }
    }
}