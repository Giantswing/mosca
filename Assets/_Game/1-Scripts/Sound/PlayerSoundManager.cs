using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    //[SerializeField] private AudioEventSO flyDashSound;

    //[SerializeField] private AudioEventSO flyDodgeSound;

    //[SerializeField] private AudioEventSO flyWallHitSound;

    [Space(15)] [SerializeField] private AudioSource flyIdleSound;
    //[SerializeField] private AudioSource flySounds;


    [SerializeField] private PlayerMovement pM;


    private void Update()
    {
        flyIdleSound.pitch = 1f + pM.inputDirection.magnitude / 3;
    }
}