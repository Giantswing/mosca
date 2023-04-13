using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class HoldablePickup : PickUpBase
{
    [Title("Holdable Properties")] public ItemHolder itemHolder;

    [HorizontalGroup("Properties")] public bool opensDoor = false;
    [HorizontalGroup("Properties")] public bool isThrowable = false;

    [HorizontalGroup("Effects", LabelWidth = 90f)]
    public SoundListAuto resetSound;

    [HorizontalGroup("Effects")] public FXListAuto resetFX;

    [HorizontalGroup("UseEffects", LabelWidth = 90f)]
    public SoundListAuto useSound;

    [HorizontalGroup("UseEffects")] public FXListAuto useFX;

    public bool hasPickUpEvent = false;
    [ShowIf("hasPickUpEvent")] public UnityEvent onPickUpEvent;

    private Vector3 _startPosition;
    private Transform _parentTransform;
    private bool _hasParent = false;

    protected override void Initialize()
    {
        base.Initialize();

        _startPosition = transform.position;
        _parentTransform = transform.parent;

        if (_parentTransform != null) _hasParent = true;
    }

    public override void EndCollect()
    {
        base.EndCollect();
        itemHolder = whoReceivesPickup.GetComponent<ItemHolder>();
        Hold(whoReceivesPickup);
    }

    public void Hold(Transform target)
    {
        itemHolder.AddItem(this);
        transform.parent = target;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        collider.enabled = false;

        if (hasPickUpEvent)
            onPickUpEvent?.Invoke();
    }

    public void Release(bool delayed = false)
    {
        itemHolder.RemoveItem(this);

        if (!delayed)
        {
            rb.isKinematic = false;
            transform.parent = null;
        }
    }

    public void Reset()
    {
        displayObject.DOShakePosition(0.45f, 1f, 10, 90, false);
        displayObject.DOShakeScale(1f, 0.3f, 10, 90, false).onComplete += () => { ImmediateReset(); };
    }

    public void ImmediateReset()
    {
        Release();
        transform.localScale = Vector3.zero;
        FXMaster.SpawnFX(transform.position, (int)resetFX);
        SoundMaster.PlaySound(transform.position, (int)resetSound, true);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = false;
        rb.useGravity = false;
        transform.parent = null;
        isFollowing = false;

        if (!_hasParent)
        {
            transform.position = _startPosition;
        }
        else
        {
            transform.SetParent(_parentTransform);
            transform.position = _parentTransform.position;
        }

        transform.rotation = Quaternion.identity;
        collider.enabled = true;
        whoToFollow = null;
        whoReceivesPickup = null;

        DOVirtual.DelayedCall(2f, () =>
        {
            transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);

            isPickedUp = false;
        });
    }


    public void Use()
    {
        FXMaster.SpawnFX(transform.position, (int)useFX);
        SoundMaster.PlaySound(transform.position, (int)useSound, true);
    }

    public virtual void Throw(Vector3 throwDirection, Collider colliderToIgnore)
    {
    }
}