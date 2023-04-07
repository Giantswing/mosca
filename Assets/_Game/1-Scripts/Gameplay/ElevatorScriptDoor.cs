using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ElevatorScriptDoor : MonoBehaviour
{
    [SerializeField] private float doorHeight = 2.9f;

    public Ease easeOpen = Ease.InOutQuad;
    public Ease easeClose = Ease.InOutQuad;

    private ParticleSystem closingFx;

    [SerializeField] private SimpleAudioEvent doorClosingSound;
    [SerializeField] private SimpleAudioEvent doorHitSound;

    private Queue<TweenCallback> _tweens = new();
    private Vector3 openPos;
    private Vector3 closePos;
    private bool isInElevator;
    public bool isDoorOpen;
    private bool canDoSound = false;

    private void Start()
    {
        easeOpen = Ease.InOutQuad;
        easeClose = Ease.OutBounce;
        closingFx = GetComponent<ParticleSystem>();

        DOVirtual.DelayedCall(0.5f, () => canDoSound = true);


        if (isDoorOpen)
        {
            openPos = transform.localPosition;
            closePos = transform.localPosition - new Vector3(0, doorHeight, 0);
        }
        else
        {
            closePos = transform.localPosition;
            openPos = transform.localPosition + new Vector3(0, doorHeight, 0);
        }


        isInElevator = false;
        if (transform.parent != null)
            if (transform.parent.name.ToLower().Contains("elevator"))
                isInElevator = true;
    }

    public void OpenDoor()
    {
        if (isDoorOpen == false)
        {
            transform.DOComplete();
            doorClosingSound.pitch.minValue = 1.2f;
            doorClosingSound.pitch.maxValue = 1.4f;


            isDoorOpen = true;

            /*
            if (isInElevator)
                transform.DOLocalMoveY(0, 1f).SetEase(easeOpen);
            else
                transform.DOLocalMoveY(transform.localPosition.y + doorHeight, 1f).SetEase(easeOpen);
                */

            _tweens.Enqueue(transform.DOLocalMoveY(openPos.y, 1f).SetEase(easeOpen).onComplete +=
                OnCompleteAnimation);

            if (canDoSound)
                SoundMaster.PlaySound(transform.position, (int)SoundList.DoorClosing, "", true);
        }
    }

    private void OnCompleteAnimation()
    {
        //check if there are more tweens to play
        if (_tweens.Count > 0)
            //play the next tween
            _tweens.Dequeue().Invoke();
    }


    public void CloseDoor()
    {
        if (isDoorOpen)
        {
            transform.DOComplete();
            doorClosingSound.pitch.minValue = 0.9f;
            doorClosingSound.pitch.maxValue = 1.1f;

            isDoorOpen = false;

            _tweens.Enqueue(transform.DOLocalMoveY(closePos.y, 1f).SetEase(easeClose).onComplete +=
                OnCompleteAnimation);

            if (canDoSound)
                SoundMaster.PlaySound(transform.position, (int)SoundList.DoorClosing, "", true);

            transform.DOLocalMoveX(transform.localPosition.x, 0.4f).onComplete += () =>
            {
                closingFx.Emit(20);
                if (canDoSound)
                    SoundMaster.PlaySound(transform.position, (int)SoundList.DoorHit, "", true);
            };
        }
    }


    public void ToggleDoor()
    {
        if (isDoorOpen)
            CloseDoor();
        else
            OpenDoor();
    }
}