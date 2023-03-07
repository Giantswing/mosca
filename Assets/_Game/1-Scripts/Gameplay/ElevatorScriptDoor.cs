using System;
using DG.Tweening;
using UnityEngine;

public class ElevatorScriptDoor : MonoBehaviour
{
    public bool isDoorOpen;
    [SerializeField] private float doorHeight = 2.9f;

    public Ease easeOpen = Ease.InOutQuad;
    public Ease easeClose = Ease.InOutQuad;

    private void Start()
    {
        easeOpen = Ease.InCirc;
        easeClose = Ease.OutBounce;
    }

    public void OpenDoor()
    {
        if (isDoorOpen == false)
        {
            isDoorOpen = true;
            transform.DOLocalMoveY(doorHeight, 1f).SetEase(easeOpen);
        }
    }


    public void CloseDoor()
    {
        if (isDoorOpen)
        {
            isDoorOpen = false;
            transform.DOLocalMoveY(-doorHeight, 1f).SetEase(easeClose);
        }
    }

    public void CloseDoor(Action callback)
    {
        if (isDoorOpen)
        {
            isDoorOpen = false;
            transform.DOLocalMoveY(-doorHeight, 1f).SetEase(easeClose).OnComplete(() => callback());
        }
    }
}