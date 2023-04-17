using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Attributes))]
public class PlayerIdentifier : MonoBehaviour
{
    public PlayerDataSO playerReference;

    public MovementSystem movementSystem;
    public DashAbility dashAbility;
    public ChargeSystem chargeSystem;
    public InputReceiver inputReceiver;
    public Attributes attributes;
    public FlipSystem flipSystem;

    private void Awake()
    {
        playerReference.attributes = GetComponent<Attributes>();

        movementSystem = GetComponent<MovementSystem>();
        dashAbility = GetComponent<DashAbility>();
        chargeSystem = GetComponent<ChargeSystem>();
        inputReceiver = GetComponent<InputReceiver>();
        attributes = GetComponent<Attributes>();
        flipSystem = GetComponent<FlipSystem>();
    }

    private void Start()
    {
        DisableMovement();
        gameObject.SetActive(false);
        DOVirtual.DelayedCall(0.5f, () => FXMaster.SpawnFX(transform.position, (int)FXListAuto.Reset));
        DOVirtual.DelayedCall(1.5f, () =>
        {
            FXMaster.SpawnFX(transform.position, (int)FXListAuto.Reset);
            gameObject.SetActive(true);
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