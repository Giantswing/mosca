using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(LookAtRotation))]
public class ZDepthSystem : MonoBehaviour
{
    private WaitForSeconds wait = new(0.01f);
    public LayerMask backgroundLayer;
    private float zDepth, zDepthTo;
    private float zDepthOffset = 1f;
    private Vector3 lastLocalPosInsideLevel;
    private float _zRot, _zRotTo;
    private LookAtRotation lookAtRotation;


    private void Awake()
    {
        lookAtRotation = GetComponent<LookAtRotation>();
    }

    private void Start()
    {
        DOVirtual.DelayedCall(1f, () => StartCoroutine(UpdateZDepth()));
    }

    private IEnumerator UpdateZDepth()
    {
        while (true)
        {
            var foundWall = false;
            _zRotTo = 0;

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 6f,
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
                position = transform.position + transform.forward * (zDepth - zDepthOffset);
                transform.position = position;

                Quaternion rotationToLook = Quaternion.LookRotation(-hit.normal, Vector3.up);
                _zRotTo = rotationToLook.eulerAngles.y;
            }

            _zRot = Mathf.Lerp(_zRot, _zRotTo, Time.deltaTime * 30f);

            lookAtRotation.depthRotation = _zRot;

            yield return wait;
        }
    }
}