using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

[RequireComponent(typeof(Attributes))]
[RequireComponent(typeof(FlipSystem))]
[RequireComponent(typeof(Rigidbody))]
public class LookAtRotation : MonoBehaviour
{
    private Attributes attributes;
    private float _modelRotation;
    private FlipSystem flipSystem;
    private Rigidbody rb;
    public float depthRotation = 0f;
    public float angleRotation = 0f;
    private float rotateSpeed = 7f;
    public bool useAngleRotation = false;
    public bool useForcedRotation = false;

    private void Awake()
    {
        attributes = GetComponent<Attributes>();
        flipSystem = GetComponent<FlipSystem>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        rotateSpeed = 12f;

        if (useForcedRotation) return;

        if (useAngleRotation)
        {
            rotateSpeed = 8f;
            angleRotation = Mathf.LerpAngle(_modelRotation,
                Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg -
                180 * (flipSystem.flipDirection == 1 ? 0 : 1), .9f);
        }
        else
        {
            rotateSpeed = 5f;
            angleRotation = Mathf.LerpAngle(_modelRotation,
                Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg -
                180 * (flipSystem.flipDirection == 1 ? 0 : 1), .2f);
        }

        if (rb.velocity.magnitude < 0.45f)
        {
            rotateSpeed = 1f;
            angleRotation = Mathf.LerpAngle(_modelRotation,
                0, .001f);
        }


        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, depthRotation, angleRotation),
            Time.deltaTime * rotateSpeed); //15f
    }

    public void SetForcedRotation(Vector3 direction, float amount)
    {
        useForcedRotation = true;

        if (direction.x > 0)
            flipSystem.Flip(1);
        else
            flipSystem.Flip(-1);

        rotateSpeed = 5f;
        angleRotation = Mathf.LerpAngle(_modelRotation,
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg -
            180 * (flipSystem.flipDirection == 1 ? 0 : 1), 1);


        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, depthRotation, angleRotation),
            Time.deltaTime * rotateSpeed); //15f
    }

    public void ResetForcedRotation()
    {
        useForcedRotation = false;
    }
}