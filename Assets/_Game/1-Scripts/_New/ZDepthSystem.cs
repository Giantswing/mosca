using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(LookAtRotation))]
public class ZDepthSystem : MonoBehaviour
{
    //private WaitForSeconds wait = new(0.01f);
    public LayerMask backgroundLayer;
    private float zDepth, zDepthTo;
    [SerializeField] private float zDepthOffset = 1f;
    private Vector3 lastLocalPosInsideLevel;
    private float _zRot, _zRotTo;
    private LookAtRotation lookAtRotation;
    private bool startChecking = false;


    private enum Direction
    {
        Forward,
        Right,
        Up
    }

    [SerializeField] private Direction offsetDirection = Direction.Forward;
    [SerializeField] private bool forceVector3Forward = false;
    private Vector3 forwardVector;


    private void Awake()
    {
        lookAtRotation = GetComponent<LookAtRotation>();
        startChecking = false;

        switch (offsetDirection)
        {
            case Direction.Forward:
                forwardVector = transform.forward;
                break;

            case Direction.Right:
                forwardVector = transform.right;
                break;

            case Direction.Up:
                forwardVector = transform.up;
                break;
        }

        DOVirtual.DelayedCall(.3f, () => startChecking = true);
    }

    private Vector3 ReturnProperForwardVector()
    {
        Vector3 result = transform.forward;

        if (forceVector3Forward == false)
            switch (offsetDirection)
            {
                case Direction.Forward:
                    result = transform.forward;
                    break;

                case Direction.Right:
                    result = transform.right;
                    break;

                case Direction.Up:
                    result = transform.up;
                    break;
            }
        else result = Vector3.forward;

        return result;
    }

    private void Update()
    {
        if (!enabled || !startChecking) return;
        var foundWall = false;
        _zRotTo = 0;

        if (Physics.Raycast(transform.position, ReturnProperForwardVector(), out RaycastHit hit, 6f,
                backgroundLayer))
        {
            zDepthTo = hit.distance;
            lastLocalPosInsideLevel = transform.localPosition;
            foundWall = true;
        }

        zDepth = Mathf.Lerp(zDepth, zDepthTo, Time.deltaTime * 50f);

        if (foundWall)
        {
            Vector3 position = transform.position;
            position = transform.position + ReturnProperForwardVector() * (zDepth - zDepthOffset);
            transform.position = position;

            Quaternion rotationToLook = Quaternion.LookRotation(-hit.normal, Vector3.up);
            _zRot = rotationToLook.eulerAngles.y;
        }

        //_zRot = Mathf.Lerp(_zRot, _zRotTo, Time.deltaTime * 30f);

        lookAtRotation.depthRotation = _zRot;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, ReturnProperForwardVector() * 6f);
    }
}