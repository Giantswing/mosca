using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Attributes))]
[RequireComponent(typeof(FlipSystem))]
[RequireComponent(typeof(LookAtRotation))]
public class ChargeSystem : MonoBehaviour
{
    public int chargeShot = 0;
    [SerializeField] private float chargeAmount = 0;
    [SerializeField] private float chargeSpeed = 1f;
    [SerializeField] private float maxChargeAmount = 1f;
    [SerializeField] private float chargeOffset = 0.5f;
    private Crown crown;
    private Attributes attributes;
    private FlipSystem flipSystem;
    private LookAtRotation lookAtRotation;

    public UnityEvent OnCharge;
    public UnityEvent<string, int> ChargeAnimation;
    public UnityEvent<Vector3, float> OnChargeDirection;
    public UnityEvent OnRelease;
    public UnityEvent<Vector3, float> OnReleaseDirection;

    private Vector3 shootDir;

    private void Awake()
    {
        crown = GetComponentInChildren<Crown>();
        attributes = GetComponent<Attributes>();
        flipSystem = GetComponent<FlipSystem>();
    }

    private void Start()
    {
        crown.SetUpIgnoreCollisions(attributes.hardCollider);
    }

    public void ChangeChargeDirection(Vector3 direction)
    {
        shootDir = fixEmptyDirection(direction);
    }

    private Vector3 fixEmptyDirection(Vector3 direction)
    {
        Vector3 result = direction;

        if (direction.magnitude < 0.05f)
            result = Vector3.right * flipSystem.flipDirection;

        return result;
    }

    public void StartCharge(Vector3 direction)
    {
        direction = fixEmptyDirection(direction);
        OnCharge?.Invoke();
        shootDir = direction;
        if (!crown.isGrabbed) return;
        chargeShot = 1;
        //chargeSound.Play(chargeSoundSource);
        crown.ChangeCrownPos(0);
        ChargeAnimation?.Invoke("ChargingShot", chargeShot);
        //playerAnimationHandler.SetChargingShot(chargeShot);
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
        }

        if (chargeShot == 1)
        {
            OnChargeDirection?.Invoke(shootDir, chargeAmount);
            crown.UpdateMaterial(chargeAmount, crown.glowColor);
        }
    }

    public void Shoot(Vector3 direction)
    {
        if (chargeShot != 1) return;

        //chargeSoundSource.Stop();
        Vector3 finalShotDir = shootDir.normalized;
        if (finalShotDir == Vector3.zero) finalShotDir = Vector3.right * flipSystem.flipDirection;

        DOVirtual.DelayedCall(0.1f, () => { OnRelease?.Invoke(); });

        OnReleaseDirection?.Invoke(finalShotDir, chargeAmount);

        chargeShot = 2;
        ChargeAnimation?.Invoke("ChargingShot", chargeShot);
        //playerAnimationHandler.SetChargingShot(chargeShot);
        DOVirtual.DelayedCall(0.1f, () => { crown.Throw(finalShotDir, chargeOffset + chargeAmount); });

        DOVirtual.DelayedCall(0.5f, () =>
        {
            chargeShot = 0;
            ChargeAnimation?.Invoke("ChargingShot", chargeShot);
            //playerAnimationHandler.SetChargingShot(chargeShot);
        });
    }
}