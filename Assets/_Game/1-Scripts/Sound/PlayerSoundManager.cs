using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    [Header("Audio Events")] [SerializeField]
    private AudioEventSO flyDashSound;

    [SerializeField] private AudioEventSO flyDodgeSound;

    [SerializeField] private AudioEventSO flyWallHitSound;

    [Space(15)] [SerializeField] private AudioSource flyIdleSound;
    [SerializeField] private AudioSource flySounds;


    [SerializeField] private PlayerMovement pM;

    public void PlayDashSound()
    {
        flyDashSound.Play(flySounds);
    }

    public void PlayDodgeSound()
    {
        flyDodgeSound.Play(flySounds);
    }

    public void PlayWallHitSound()
    {
        GlobalAudioManager.PlaySound(flyWallHitSound, transform.position);
    }

    private void Update()
    {
        flyIdleSound.pitch = 1f + pM.inputDirection.magnitude / 3;
    }
}