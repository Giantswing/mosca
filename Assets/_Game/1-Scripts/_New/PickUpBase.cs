using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class PickUpBase : MonoBehaviour, IPickUp
{
    [HideInInspector] public Rigidbody rb { get; set; }
    [HideInInspector] public Collider collider { get; set; }
    [HideInInspector] public Transform whoToFollow { get; set; }
    [HideInInspector] public Transform whoReceivesPickup { get; set; }
    [HideInInspector] public bool isPickedUp { get; set; }
    [HideInInspector] public bool isFollowing { get; set; }

    public enum PickUpAnimation
    {
        Rotatable,
        RotatableSlow,
        Static,
        Floating
    }

    [ReadOnly] public Transform displayObject;
    public float collectDistance = 1f;
    [HideInInspector] public bool isHoldable = false;

    [PropertySpace(SpaceBefore = 10, SpaceAfter = 10)]
    public PickUpAnimation floatingAnimation;

    public bool destroyOnCollect = true;

    [HorizontalGroup("OnCollect", LabelWidth = 90f)]
    public SoundListAuto collectSound;

    [HorizontalGroup("OnCollect")] public FXListAuto collectFX;
    [HorizontalGroup("OnCollect")] public bool follow;

    private IPickUpEffect[] effects;


    /*

    [PropertySpace(SpaceBefore = 5f, SpaceAfter = 5f)]
    public bool hasCollectEvent = false;

    [ShowIf("hasCollectEvent")] public UnityEvent<Transform> onCollectEvent;
    */


    private void Awake()
    {
        if (TryGetComponent(out HoldablePickup holdableItem))
            isHoldable = true;

        rb = GetComponent<Rigidbody>();
        displayObject = transform.GetChild(0);
        collider = GetComponent<Collider>();
        effects = GetComponents<IPickUpEffect>();
        Initialize();
    }

    private void Start()
    {
        isPickedUp = false;
        isFollowing = false;
    }

    protected virtual void Initialize()
    {
        switch (floatingAnimation)
        {
            case PickUpAnimation.Rotatable:
                displayObject.DOLocalRotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
                break;

            case PickUpAnimation.RotatableSlow:

                displayObject.DOLocalRotate(new Vector3(0, 360, 0), 8f, RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
                break;
            case PickUpAnimation.Floating:
                displayObject.DOLocalMoveY(0.2f, 1f).SetLoops(-1, LoopType.Yoyo);
                break;
        }
    }

    public void StartFollowing()
    {
        isFollowing = true;
        TimerTick.tickFrameFixed += Follow;
    }

    private void OnDisable()
    {
        TimerTick.tickFrameFixed -= Follow;
    }

    public void Follow()
    {
        Vector3 dirToMove = (whoToFollow.position - transform.position).normalized * 2f;
        Vector3 torqueVector = Vector3.Cross(dirToMove, Vector3.up) * 40f;

        rb.AddForce(dirToMove, ForceMode.Impulse);
        rb.AddTorque(torqueVector, ForceMode.Impulse);

        rb.drag += Time.fixedDeltaTime;

        if (Vector3.Distance(whoReceivesPickup.position, transform.position) < collectDistance) OnCollect();
    }

    public void OnCollect()
    {
        TimerTick.tickFrameFixed -= Follow;
        isFollowing = false;
        DOVirtual.DelayedCall(.01f, () => { rb.velocity = Vector3.zero; });

        if (!isHoldable)
            transform.DOScale(0, .1f).OnComplete(() => { EndCollect(); });
        else EndCollect();
    }

    public virtual void EndCollect()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.transform.DOLocalRotate(Vector3.zero, .1f);
        SoundMaster.PlaySound(transform.position, (int)collectSound);
        FXMaster.SpawnFX(transform.position, (int)collectFX, follow ? whoReceivesPickup : null);

        //if (hasCollectEvent) onCollectEvent.Invoke(whoReceivesPickup);

        if (effects.Length > 0)
            foreach (IPickUpEffect effect in effects)
                effect.OnCollect(whoReceivesPickup);

        if (!isHoldable)
        {
            DOTween.Kill(transform);
            DOTween.Kill(displayObject);
        }

        if (destroyOnCollect) Destroy(gameObject);
    }
}