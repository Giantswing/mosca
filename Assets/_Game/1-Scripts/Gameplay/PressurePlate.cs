using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private Material activeMaterial;
    [SerializeField] private Material inactiveMaterial;
    [SerializeField] private MeshRenderer[] meshRenderer;

    [Space(10)] public UnityEvent OnPress;
    public UnityEvent OnRelease;

    [SerializeField] private float releaseTime = 0.1f;
    [SerializeField] private bool canBeActivated = true;
    private WaitForSeconds delay = new(1f);
    private bool isActive = false;
    private Collider otherCollider;
    [SerializeField] private CableGenerator cableGenerator;


    private void Start()
    {
        foreach (var mRenderer in meshRenderer)
            mRenderer.material = inactiveMaterial;

        isActive = false;

        var eventCount = OnPress.GetPersistentEventCount();
        if (eventCount > 0)
        {
            cableGenerator.targets = new Transform[eventCount];
            for (var i = 0; i < eventCount; i++)
                cableGenerator.targets[i] = ((Component)OnPress.GetPersistentTarget(i)).transform;

            cableGenerator.CreateCable();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<IPressurePlateListener>() == null || !canBeActivated) return;

        otherCollider = other;

        foreach (var mRenderer in meshRenderer)
            mRenderer.material = activeMaterial;

        isActive = true;
        canBeActivated = false;
        OnPress.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<IPressurePlateListener>() == null || !isActive) return;

        if (otherCollider != other) return;

        canBeActivated = false;

        StartCoroutine(reActivate());
        transform.DOLocalMoveX(transform.localPosition.x, releaseTime).onComplete += () => { Disable(); };
    }

    private void Disable()
    {
        foreach (var mRenderer in meshRenderer)
            mRenderer.material = inactiveMaterial;
        OnRelease.Invoke();
    }

    private IEnumerator reActivate()
    {
        yield return delay;
        canBeActivated = true;
        isActive = false;
    }

    /*
    private void CreateCable()
    {
        //get how many events are connected to the onpress unity event
        var eventCount = OnPress.GetPersistentEventCount();

        //create as many linerenderer components as there are events and add those components to the object
        cables = new LineRenderer[eventCount];

        //for each cable, create line renderer component and assign it to the cables
        for (var i = 0; i < cables.Length; i++)
        {
            var child = new GameObject();
            child.transform.parent = transform;
            var startingPos = transform.position + new Vector3(0, 0, 0f);
            var targetPos = ((Component)OnPress.GetPersistentTarget(i)).transform.position + new Vector3(0, 0, 1f);
            cables[i] = child.gameObject.AddComponent<LineRenderer>();
            cables[i].positionCount = 2 + cableHangPoints;
            cables[i].SetPosition(0, startingPos);
            cables[i].SetPosition(1 + cableHangPoints, targetPos);

            //set the positions of the hanging points
            for (var j = 0; j < cableHangPoints; j++)
            {
                //get intermediary position between the two points
                var intermediaryPos = Vector3.Lerp(startingPos, targetPos, j / (float)cableHangPoints);

                //add vertical offset to the intermediary position, so that the cable hangs down
                var yOffset =
                    Mathf.Sin(j / (float)cableHangPoints * Mathf.PI) *
                    -hangStrength; // adjust the 0.2f value to change the hang intensity
                intermediaryPos += new Vector3(0, yOffset, 0);


                //set the position of the hanging point
                cables[i].SetPosition(1 + j, intermediaryPos);
            }


            cables[i].startWidth = 0.1f;
            cables[i].endWidth = 0.1f;

            cables[i].material = cableMaterial;
        }

        StartCoroutine(UpdateCablePositions());
    }
    */
}

public interface IPressurePlateListener
{
}