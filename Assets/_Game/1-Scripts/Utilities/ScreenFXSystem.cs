using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenFXSystem : MonoBehaviour
{
    private static ScreenFXSystem Instance;
    [SerializeField] private CinemachineVirtualCamera vcam;
    private CinemachineBasicMultiChannelPerlin vcamNoise;
    private bool _changeTimeScale;
    private float _distortionIntensity;
    private float _distortionIntensityTo;
    private bool _isFrozen;
    private float _timeScaleTo = 1f;
    private Volume _volume;
    private LensDistortion lensDistortion;

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

        if (vcam == null) vcam = FindObjectOfType<CinemachineVirtualCamera>();
        vcamNoise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        if (_changeTimeScale || Time.timeScale > 0.1f)
            Time.timeScale = _timeScaleTo != 1f ? Mathf.Lerp(Time.timeScale, _timeScaleTo, 0.8f) : 1f;

        if (_distortionIntensity < 0)
        {
            _distortionIntensity += -_distortionIntensity * Time.deltaTime * 2f;
            _distortionIntensity = Mathf.Lerp(_distortionIntensity, _distortionIntensityTo, 0.2f);
            lensDistortion.intensity.value = _distortionIntensity;
        }
    }

    public static void FreezeFrames(float duration)
    {
        if (Instance._isFrozen == false)
        {
            Instance.StartCoroutine(Instance.IEFreezeFrames(duration));
            Instance._changeTimeScale = true;
        }
    }

    public static void ShakeCamera(float duration = 1f, float strength = 1f)
    {
        Instance.StartCoroutine(Instance.IEShakeCamera(duration, strength));
    }

    public static void ShakeCameraImmediate(float strength)
    {
        Instance.vcamNoise.m_AmplitudeGain = 0.5f * strength;
        Instance.vcamNoise.m_FrequencyGain = 1f * strength;
    }

    public static void DistortView(float duration)
    {
        Instance.StartCoroutine(Instance.IEDistortView(duration));
        //test
    }

    private IEnumerator IEShakeCamera(float duration, float strength)
    {
        Instance.vcamNoise.m_AmplitudeGain = 0.5f * strength;
        Instance.vcamNoise.m_FrequencyGain = 1f * strength;
        yield return new WaitForSecondsRealtime(duration);
        Instance.vcamNoise.m_AmplitudeGain = 0f;
        Instance.vcamNoise.m_FrequencyGain = 0f;
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
}