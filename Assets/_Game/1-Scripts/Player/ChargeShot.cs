using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChargeShot : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerAnimationHandler playerAnimationHandler;
    private PlayerReceiveInput playerReceiveInput;
    private PlayerInteractionHandler playerInteractionHandler;
    private Crown crown;

    public Action<Vector3, float> Charging;
    public Action Release;

    public int chargeShot = 0;
    [SerializeField] private float chargeAmount = 0;
    [SerializeField] private float chargeSpeed = 1f;
    [SerializeField] private float maxChargeAmount = 1f;
    [SerializeField] private float chargeOffset = 0.5f;

    [SerializeField] private SimpleAudioEvent chargeSound;
    [SerializeField] private AudioSource chargeSoundSource;


    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerAnimationHandler = GetComponent<PlayerAnimationHandler>();
        playerInteractionHandler = GetComponent<PlayerInteractionHandler>();
        crown = GetComponentInChildren<Crown>();
        //crown.SetUpIgnoreCollisions(playerInteractionHandler.myColliders);
    }

    private void Start()
    {
        crown.SetUpIgnoreCollisions(playerInteractionHandler.myColliders);
    }

    private void OnEnable()
    {
        playerReceiveInput = GetComponent<PlayerReceiveInput>();
        playerReceiveInput.OnChargeShot += PrepareShot;
        playerReceiveInput.OnChargeRelease += Shoot;
    }

    private void OnDisable()
    {
        playerReceiveInput.OnChargeShot -= PrepareShot;
        playerReceiveInput.OnChargeRelease -= Shoot;
    }

    private void PrepareShot()
    {
        if (!crown.isGrabbed) return;
        chargeShot = 1;
        chargeSound.Play(chargeSoundSource);
        crown.ChangeCrownPos(0);
        playerAnimationHandler.SetChargingShot(chargeShot);
    }

    private void Update()
    {
        if (chargeShot != 0)
        {
            chargeAmount += chargeSpeed * Time.deltaTime;
            if (chargeAmount > maxChargeAmount)
                chargeAmount = maxChargeAmount;
        }
        else
        {
            chargeAmount = 0;
            //ControllerVibration.StopVibration();
        }

        if (chargeShot == 1)
        {
            Charging?.Invoke(playerMovement.inputDirection, chargeAmount);
            crown.UpdateMaterial(chargeAmount, crown.glowColor);
            //ScreenFXSystem.ShakeCameraImmediate(chargeAmount);

            //ControllerVibration.VibrateImmediate(chargeAmount * .35f);
            //Gamepad.current.SetMotorSpeeds(0.123f, 0.234f);
        }
    }

    public void Shoot()
    {
        //ScreenFXSystem.ShakeCameraImmediate(0);
        if (chargeShot != 1) return;

        chargeSoundSource.Stop();
        Vector3 finalShotDir = playerMovement.inputDirection.normalized;


        if (finalShotDir == Vector3.zero) finalShotDir = Vector3.right * playerMovement.isFacingRight;

        Release?.Invoke();


        chargeShot = 2;
        playerAnimationHandler.SetChargingShot(chargeShot);
        DOVirtual.DelayedCall(0.1f, () => { crown.Throw(finalShotDir, chargeOffset + chargeAmount); });

        DOVirtual.DelayedCall(0.5f, () =>
        {
            chargeShot = 0;
            playerMovement.hSpeed = 0;
            playerMovement.vSpeed = 0;
            playerAnimationHandler.SetChargingShot(chargeShot);
        });
    }
}