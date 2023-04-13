using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Attributes))]
[RequireComponent(typeof(FlipSystem))]
public class MovementSystem : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 direction;
    private Attributes attributes;
    private FlipSystem flipSystem;

    public enum MovementType
    {
        Simple,
        Advanced
    }

    public MovementType movementType;

    [ShowIf("movementType", Value = MovementType.Advanced)]
    public UnityEvent<string, string, Vector3> OnMove;

    [ShowIf("movementType", Value = MovementType.Simple)]
    public UnityEvent OnMoveSimple;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        attributes = GetComponent<Attributes>();
        flipSystem = GetComponent<FlipSystem>();
    }

    private void OnEnable()
    {
        TimerTick.tickEverySecondFrame += MoveEvents;
    }

    private void OnDisable()
    {
        TimerTick.tickEverySecondFrame -= MoveEvents;
    }

    public void Move(Vector3 direction)
    {
        this.direction = direction;
    }

    public void MoveEvents()
    {
        if (movementType == MovementType.Advanced)
        {
            Vector3 mappedVelocity = Vector3.zero;
            mappedVelocity.x = Mathf.InverseLerp(0, attributes.speed, Mathf.Abs(rb.velocity.x)) *
                               Mathf.Sign(rb.velocity.x) * flipSystem.flipDirection;
            mappedVelocity.y = Mathf.InverseLerp(0, attributes.speed, Mathf.Abs(rb.velocity.y)) *
                               Mathf.Sign(rb.velocity.y);


            OnMove?.Invoke("FlyAnimSpeedH", "FlyAnimSpeedV", mappedVelocity);
        }
        else if (rb.velocity.magnitude > 0.1f)
        {
            OnMoveSimple?.Invoke();
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(direction * attributes.acceleration);

        rb.velocity = Vector3.Slerp(rb.velocity, Vector3.ClampMagnitude(rb.velocity, attributes.speed),
            Time.fixedDeltaTime * attributes.speed);
    }
}