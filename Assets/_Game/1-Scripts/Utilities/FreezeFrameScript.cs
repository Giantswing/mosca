using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FreezeFrameScript : MonoBehaviour
{
    private static FreezeFrameScript Instance;
    private bool _isFrozen = false;
    private float _timeScaleTo = 1f;
    private LensDistortion lensDistortion;
    private float _distortionIntensity = 0f;
    private float _distortionIntensityTo = 0f;
    private Volume _volume;
    private bool _changeTimeScale = false;

    private void Start()
    {
        _volume = GetComponent<Volume>();

        _isFrozen = false;
        _volume.profile.TryGet(out lensDistortion);


        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        _changeTimeScale = false;
    }

    public static void FreezeFrames(float duration)
    {
        if (Instance._isFrozen == false)
        {
            Instance.StartCoroutine(Instance.IEFreezeFrames(duration));
            Instance._changeTimeScale = true;
        }
    }

    public static void DistortView(float duration)
    {
        Instance.StartCoroutine(Instance.IEDistortView(duration));
    }

    //create coroutine freeze frames
    private IEnumerator IEFreezeFrames(float duration)
    {
        _isFrozen = true;
        _timeScaleTo = 0.35f;
        yield return new WaitForSecondsRealtime(duration);
        _timeScaleTo = 1f;
        _isFrozen = false;
        _changeTimeScale = false;
    }

    private IEnumerator IEDistortView(float duration)
    {
        Instance._distortionIntensityTo = -0.5f;
        Instance._distortionIntensity = -0.1f;
        yield return new WaitForSecondsRealtime(duration);
        Instance._distortionIntensityTo = 0f;
    }

    private void Update()
    {
        if (_changeTimeScale)
            Time.timeScale = _timeScaleTo != 1f ? Mathf.Lerp(Time.timeScale, _timeScaleTo, 0.2f) : 1f;

        if (_distortionIntensity < 0)
        {
            _distortionIntensity += -_distortionIntensity * Time.deltaTime * 2f;
            _distortionIntensity = Mathf.Lerp(_distortionIntensity, _distortionIntensityTo, 0.2f);
            lensDistortion.intensity.value = _distortionIntensity;
        }
    }
}