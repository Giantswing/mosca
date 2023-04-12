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
    [SerializeField] private List<Transform> targets = new();

    private bool isActive = false;
    private bool canCheck = true;

    [SerializeField] private CableGenerator cableGenerator;


    private void Start()
    {
        foreach (MeshRenderer mRenderer in meshRenderer)
            mRenderer.material = inactiveMaterial;

        isActive = false;

        int eventCount = OnPress.GetPersistentEventCount();
        if (eventCount > 0)
        {
            cableGenerator.targets = new Transform[eventCount];
            for (var i = 0; i < eventCount; i++)
                cableGenerator.targets[i] = ((Component)OnPress.GetPersistentTarget(i)).transform;

            cableGenerator.CreateCable();
        }
    }

    private void CheckActivation()
    {
        if (targets.Count == 0)
        {
            DOVirtual.DelayedCall(releaseTime, () =>
            {
                if (!isActive) return;
                isActive = false;
                SoundMaster.PlaySound(transform.position, (int)SoundListAuto.ElectricPowerOff, true);

                foreach (MeshRenderer mRenderer in meshRenderer)
                    mRenderer.material = inactiveMaterial;

                OnRelease.Invoke();
            });
        }
        else
        {
            if (isActive) return;
            isActive = true;
            SoundMaster.PlaySound(transform.position, (int)SoundListAuto.ElectricPowerUp, true);
            foreach (MeshRenderer mRenderer in meshRenderer)
                mRenderer.material = activeMaterial;

            OnPress.Invoke();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (canCheck && other.TryGetComponent(out IPressurePlateListener listener))
        {
            if (!targets.Contains(listener.transform))
                targets.Add(other.transform);

            if (!isActive)
                CheckActivation();
            /*
            canCheck = false;
            DOVirtual.DelayedCall(0.1f, () => canCheck = true);
            */
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IPressurePlateListener listener))
        {
            if (targets.Contains(listener.transform))
                targets.Remove(other.transform);

            if (isActive) CheckActivation();
        }
    }


    private void OnDrawGizmos()
    {
        int eventCount = OnPress.GetPersistentEventCount();
        if (eventCount > 0)
        {
            var targets = new Transform[eventCount];
            Gizmos.color = Color.red;

            for (var i = 0; i < eventCount; i++)
            {
                if (OnPress.GetPersistentTarget(i) == null) continue;
                targets[i] = ((Component)OnPress.GetPersistentTarget(i)).transform;

                if (targets[i] != null)
                    Gizmos.DrawLine(transform.position, targets[i].position);
            }
        }
    }
}

public interface IPressurePlateListener
{
    Transform transform { get; }
}