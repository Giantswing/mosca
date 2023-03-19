using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Jobs;
using UnityEngine;

public class DSwitcherScript : MonoBehaviour, IPressurePlateListener
{
    public bool isHorizontal = true;
    [SerializeField] private SimpleAudioEvent switchSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private bool canBeActivated = true;


    private WaitForSeconds delay = new(0.1f);

    private Sequence _mySequence;

    private float endRot = 0;

    private void Start()
    {
        var rot = Mathf.Round(transform.eulerAngles.z);
        if (rot == 0 || rot % 180 == 0)
            isHorizontal = true;
        else
            isHorizontal = false;

        _mySequence = DOTween.Sequence();
        endRot = transform.eulerAngles.z;
    }

    public void Hit(Vector3 otherPos)
    {
        if (isHorizontal)
        {
            if (otherPos.y > transform.position.y)
                if (otherPos.x < transform.position.x)
                    Rotate(1);
                else
                    Rotate(-1);
            else if (otherPos.x < transform.position.x)
                Rotate(-1);
            else
                Rotate(1);
        }

        else if (!isHorizontal)
        {
            if (otherPos.x < transform.position.x)
            {
                if (otherPos.y > transform.position.y)
                    Rotate(-1);
                else
                    Rotate(1);
            }

            else if (otherPos.x >= transform.position.x)
            {
                if (otherPos.y > transform.position.y)
                    Rotate(1);
                else Rotate(-1);
            }
        }
    }

    public void Hit()
    {
        Rotate(1);
    }

    public void HitReverse()
    {
        Rotate(-1);
    }

    private void Rotate(int clockwise)
    {
        if (canBeActivated == false)
            return;

        endRot += 90 * clockwise;

        _mySequence.Append(transform.DORotate(new Vector3(0, 0, endRot), 1.35f)
            .SetEase(Ease.OutBounce));

        /*
         _mySequence.Play();
         transform.DOComplete();
        transform.DORotate(new Vector3(0, 0, transform.localRotation.eulerAngles.z + 90 * clockwise), 1.35f)
            .SetEase(Ease.OutBounce);*/

        isHorizontal = !isHorizontal;
        switchSound.Play(audioSource);
        canBeActivated = false;

        StartCoroutine(reActivate());
    }

    private IEnumerator reActivate()
    {
        yield return delay;
        canBeActivated = true;
    }
}