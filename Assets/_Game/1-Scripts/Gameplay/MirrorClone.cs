using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class MirrorClone : MonoBehaviour, IPressurePlateListener
{
    public PlayerReferenceSO playerReference;
    [SerializeField] private Transform my3DModel;
    [ReadOnly] public Mirror parentMirror;
    private Rigidbody rb;
    private Collider col;
    private Vector2 mirrorInput;
    private STATS myStats;

    [SerializeField] private float forceMultiplier = 1f;
    [SerializeField] private float bumpForce = 1000f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        myStats = GetComponent<STATS>();
        col = GetComponent<Collider>();
    }

    private void Start()
    {
        MoveToMirroredPosition();
    }

    private void OnEnable()
    {
        TimerTick.tickEveryHalfOfSecond += DrawVelocityRay;
    }

    private void Update()
    {
        UpdateStats();
    }

    private void FixedUpdate()
    {
        UpdateRigidbodyMirrored();
    }

    private void UpdateRigidbodyMirrored()
    {
        Vector3 newVelocity = Vector3.zero;

        newVelocity = new Vector3(-playerReference.playerMovement.velocity.x,
            playerReference.playerMovement.velocity.y,
            -playerReference.playerMovement.velocity.z);

        rb.AddForce(newVelocity * forceMultiplier);
    }

    private void UpdateStats()
    {
        myStats.ST_CanDoDmg = playerReference.playerStats.ST_CanDoDmg;
    }

    public void MoveToMirroredPosition()
    {
        Vector3 result = Vector3.zero;

        // Calculate the mirror position
        result = parentMirror.transform.position +
                 (parentMirror.transform.position - playerReference.playerTransform.position);

        result.y = playerReference.playerTransform.position.y;

        transform.position = result;
        FXMaster.SpawnFX(transform.position, (int)FXListAuto.Reset);
        transform.DOPunchScale(Vector3.one * 0.5f, 0.5f, 1, 0.5f);
    }


    private void DrawVelocityRay()
    {
        Debug.DrawRay(transform.position, rb.velocity, Color.red, 1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 reflectDir = Vector3.Reflect(rb.velocity, collision.contacts[0].normal).normalized;
        Debug.DrawRay(collision.contacts[0].point, reflectDir, Color.green, 1f);

        //rb.AddForce(reflectDir * bumpForce);


        if (collision.gameObject.TryGetComponent(out IGenericInteractable interactable) &&
            playerReference.playerDash.isDashing)
        {
            interactable.Interact(transform.position);
            return;
        }
    }
}