using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Attributes))]
public class PlayerIdentifier : MonoBehaviour
{
    public MovementSystem movementSystem;
    public DashAbility dashAbility;
    public ChargeSystem chargeSystem;
    public InputReceiver inputReceiver;
    public Attributes attributes;
    public FlipSystem flipSystem;
    public PlayerInput playerInput;

    private void Awake()
    {
        movementSystem = GetComponent<MovementSystem>();
        dashAbility = GetComponent<DashAbility>();
        chargeSystem = GetComponent<ChargeSystem>();
        inputReceiver = GetComponent<InputReceiver>();
        attributes = GetComponent<Attributes>();
        flipSystem = GetComponent<FlipSystem>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        if (TargetGroupControllerSystem.Instance.playerList.Count == 0)
        {
            StartUpAnimation();
        }
        else
        {
            DisableMovement();
            transform.localScale = Vector3.zero;
            transform.position = TargetGroupControllerSystem.Instance.playerList[0].attributes.transform.position +
                                 Vector3.right * 2f;
            DOVirtual.DelayedCall(0.05f, StartUpAnimation).SetUpdate(false);
        }
    }


    private void StartUpAnimation()
    {
        transform.localScale = Vector3.zero;
        DisableMovement();
        DOVirtual.DelayedCall(0.5f, () =>
        {
            transform.position = TargetGroupControllerSystem.Instance.playerList[0].attributes.transform.position +
                                 Vector3.right * 2f;
            FXMaster.SpawnFX(transform.position, (int)FXListAuto.Reset);
        });

        DOVirtual.DelayedCall(1.5f, () =>
        {
            FXMaster.SpawnFX(transform.position, (int)FXListAuto.Reset);
            SoundMaster.PlaySound(transform.position, (int)SoundListAuto.SimplePop);

            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.95f).SetEase(Ease.OutElastic);
            EnableMovement();
        });
    }

    public void DisableMovement()
    {
        movementSystem.enabled = false;
        dashAbility.enabled = false;
        chargeSystem.enabled = false;
        //inputReceiver.enabled = false;
    }

    public void EnableMovement()
    {
        movementSystem.enabled = true;
        dashAbility.enabled = true;
        chargeSystem.enabled = true;
        //inputReceiver.enabled = true;
    }
}