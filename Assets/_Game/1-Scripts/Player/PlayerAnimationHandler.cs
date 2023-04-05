using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private static readonly int FlyAnimSpeedH = Animator.StringToHash("FlyAnimSpeedH");
    private static readonly int FlyAnimSpeedV = Animator.StringToHash("FlyAnimSpeedV");
    private static readonly int IsDashing = Animator.StringToHash("IsDashing");
    private static readonly int IsDoubleDashing = Animator.StringToHash("IsDoubleDashing");
    private static readonly int IsDodging = Animator.StringToHash("IsDodging");
    private static readonly int ChargingShot = Animator.StringToHash("ChargingShot");
    private Animator flyAnimator;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        flyAnimator = GetComponentInChildren<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }


    private void Update()
    {
        flyAnimator.SetFloat(FlyAnimSpeedH, playerMovement.inputDirection.x * playerMovement.isFacingRight);
        flyAnimator.SetFloat(FlyAnimSpeedV, playerMovement.inputDirection.y);
    }

    public void SetIsDashing(bool isDashing)
    {
        if (isDashing == false)
        {
            flyAnimator.SetBool(IsDashing, false);
        }
        else
        {
            //check if the animator is already playing the clip fly_dash
            if (flyAnimator.GetCurrentAnimatorStateInfo(0).IsName("fly_dash"))
                //if it is, force to restart the clip
                flyAnimator.Play("fly_dash", -1, 0);
            else
                //if it isn't, then we can just set the bool to true
                flyAnimator.SetBool(IsDashing, true);
        }
    }


    public void SetIsDoubleDashing(bool isDoubleDashing)
    {
        flyAnimator.SetBool(IsDoubleDashing, isDoubleDashing);
    }

    public void SetIsDodging(bool isDodging)
    {
        flyAnimator.SetBool(IsDodging, isDodging);
    }

    public void SetChargingShot(int chargingShot)
    {
        flyAnimator.SetInteger(ChargingShot, chargingShot);
    }
}