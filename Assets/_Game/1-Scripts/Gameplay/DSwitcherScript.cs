using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Jobs;
using UnityEngine;

public class DSwitcherScript : MonoBehaviour
{
    public bool isHorizontal = true;
    [SerializeField] private SimpleAudioEvent switchSound;
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        if (transform.localRotation.eulerAngles.z == 0)
            isHorizontal = true;
        else
            isHorizontal = false;
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
        transform.DORotate(new Vector3(0, 0, transform.localRotation.eulerAngles.z + 90 * clockwise), 1.35f)
            .SetEase(Ease.OutBounce);
        isHorizontal = !isHorizontal;
        switchSound.Play(audioSource);
    }
}