using System.Collections;
using System.Collections.Generic;
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

    private void Awake()
    {
        attributes = GetComponent<Attributes>();
        flipSystem = GetComponent<FlipSystem>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
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
}