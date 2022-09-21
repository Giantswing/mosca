using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeFrameScript : MonoBehaviour
{
    private static FreezeFrameScript Instance;
    private bool _isFrozen = false;
    private float _timeScaleTo = 1f;

    private void Start()
    {
        _isFrozen = false;

        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public static void FreezeFrames(float duration)
    {
        if (Instance._isFrozen == false)
            Instance.StartCoroutine(Instance.IEFreezeFrames(duration));
    }

    //create coroutine freeze frames
    private IEnumerator IEFreezeFrames(float duration)
    {
        _isFrozen = true;
        _timeScaleTo = 0.25f;
        yield return new WaitForSecondsRealtime(duration);
        _timeScaleTo = 1f;
        _isFrozen = false;
    }

    private void Update()
    {
        Time.timeScale = _timeScaleTo != 1f ? Mathf.Lerp(Time.timeScale, _timeScaleTo, 0.5f) : 1f;
    }
}