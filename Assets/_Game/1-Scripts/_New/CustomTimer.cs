using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Events;

public class CustomTimer : MonoBehaviour
{
    public float totalTime = 5f;
    public float lifeTime = 5f;
    public UnityEvent OnTimerEnd;

    public bool useCustomBeepMaterial = false;
    [ShowIf("useCustomBeepMaterial")] public float beepDuration = 0.5f;
    [ShowIf("useCustomBeepMaterial")] public Material customBeepMaterial;

    [ShowIf("useCustomBeepMaterial")] [ReadOnly]
    public Material defaultMaterial;

    [ShowIf("useCustomBeepMaterial")] [ReadOnly]
    public MeshRenderer[] objectMeshRenderers;


    public bool useBeepSound;
    [ShowIf("useBeepSound")] public SoundListAuto beepSound;
    private SimpleAudioEvent beepAudioEvent;
    private bool isActive = false;

    private WaitForSeconds waitBeepDuration;
    private WaitForSeconds _slowFlash = new(0.5f);
    private WaitForSeconds _midFlash = new(0.25f);
    private WaitForSeconds _fastFlash = new(0.1f);

    private void Awake()
    {
        objectMeshRenderers = GetComponentsInChildren<MeshRenderer>();
        if (useBeepSound)
            beepAudioEvent = SoundMaster.GetAudioEvent((int)beepSound);

        waitBeepDuration = new WaitForSeconds(beepDuration);

        defaultMaterial = objectMeshRenderers[0].sharedMaterial;
    }

    public void StartTimer()
    {
        lifeTime = totalTime;
        StartCoroutine(Beep());
        isActive = true;
    }


    public void Update()
    {
        if (isActive)
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0f)
                Complete();
        }
    }


    public void DisableTimer()
    {
        isActive = false;
        StopAllCoroutines();

        if (useCustomBeepMaterial)
            foreach (MeshRenderer renderer in objectMeshRenderers)
                renderer.material = defaultMaterial;
    }

    public void Complete()
    {
        OnTimerEnd?.Invoke();
        DisableTimer();
    }

    private IEnumerator Beep()
    {
        if (useCustomBeepMaterial)
            foreach (MeshRenderer renderer in objectMeshRenderers)
                renderer.sharedMaterial = customBeepMaterial;

        if (useBeepSound)
        {
            beepAudioEvent.pitch.minValue = Mathf.Lerp(1.2f, .8f, lifeTime / totalTime);
            beepAudioEvent.pitch.maxValue = beepAudioEvent.pitch.minValue;
            SoundMaster.PlaySound(transform.position, (int)SoundListAuto.BombBeep, true);
        }

        yield return waitBeepDuration;

        if (useCustomBeepMaterial)
            foreach (MeshRenderer renderer in objectMeshRenderers)
                renderer.material = defaultMaterial;

        if (lifeTime > totalTime / 2f)
            yield return _slowFlash;
        else if (lifeTime > totalTime / 3.5f)
            yield return _midFlash;
        else
            yield return _fastFlash;

        if (isActive)
            StartCoroutine(Beep());
    }
}