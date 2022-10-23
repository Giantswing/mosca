using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private PlayerMovement pM;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [SerializeField] private float defaultCameraZOffset = -7f;
    [SerializeField] private float defaultCameraTrackingHorInfluence = 0.2f;
    private CinemachineTransposer _virtualCameraTransposer;
    private CinemachineComposer _virtualCameraComposer;

    [SerializeField] private float defaultCameraSideAngleStrength = .7f;

    public static UnityAction<bool> OnMapToggle;

    private bool _mapOpen = false;

    [SerializeField] private Transform playerFollower;


    //CAMERA OFFSETS ////////
    private float _horCameraOffset;
    private float _horCameraOffsetTo;
    private float _vertCameraOffset;
    private float _vertCameraOffsetTo;
    private float _zoomCameraOffset;
    private float _zoomCameraOffsetTo;
    [HideInInspector] public float closeUpOffset = 0;
    [HideInInspector] public float closeUpOffsetTo = 0;

    [Header("Camera Zone")] [Space(5)] [SerializeField]
    private CameraZone currentCameraZone;

    private Vector3 _cameraZoneOffset;
    private float _cameraZoneZoom;
    private float _cameraSideAngleStrengthTo;

    private float _cameraSideAngleStrength;

    /***********************************/

    private void Start()
    {
        virtualCamera = GameObject.Find("VirtualCamera")
            .GetComponent<CinemachineVirtualCamera>();
        _virtualCameraTransposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        _virtualCameraComposer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
        _cameraSideAngleStrengthTo = 1;
        _cameraSideAngleStrength = 1;
    }

    public void UpdateCameraZone(CameraZone newCameraZone)
    {
        currentCameraZone = newCameraZone;

        if (newCameraZone != null)
        {
            _cameraZoneOffset = newCameraZone.cameraOffset;
            _cameraZoneZoom = newCameraZone.cameraZoom;
            _cameraSideAngleStrengthTo = newCameraZone.sideAngleStrength;
        }
        else
        {
            _cameraZoneOffset = Vector3.zero;
            _cameraZoneZoom = 0;
            _cameraSideAngleStrengthTo = 1;
        }
    }

    private void UpdatePlayerFollower()
    {
        playerFollower.position =
            transform.position + new Vector3(_horCameraOffset, _vertCameraOffset, 0);
    }

    private void CalculateCameraOffset()
    {
        _cameraSideAngleStrength = Mathf.Lerp(_cameraSideAngleStrength, _cameraSideAngleStrengthTo * pM.isFacingRight,
            1f * Time.deltaTime);

        _horCameraOffsetTo = pM.hSpeed + 3f * pM.isFacingRight + _cameraZoneOffset.x;
        _horCameraOffset += (_horCameraOffsetTo - _horCameraOffset) * Time.deltaTime * 0.5f;

        _vertCameraOffsetTo = pM.vSpeed * 2.5f - _cameraZoneOffset.y;
        _vertCameraOffset += (_vertCameraOffsetTo - _vertCameraOffset) * Time.deltaTime * 2f;


        _zoomCameraOffsetTo = pM.inputDirection.magnitude * 2.5f + _cameraZoneZoom * -3f;
        _zoomCameraOffset += (_zoomCameraOffsetTo - _zoomCameraOffset) * Time.deltaTime * 0.7f;

        closeUpOffset += (closeUpOffsetTo - closeUpOffset) * Time.deltaTime * 1f;

        if (closeUpOffsetTo > 0)
            closeUpOffsetTo -= Time.deltaTime * 2f;


        _virtualCameraTransposer.m_FollowOffset =
            new Vector3(-4f * defaultCameraSideAngleStrength * _cameraSideAngleStrength, 0,
                defaultCameraZOffset - _zoomCameraOffset + Mathf.Abs(_cameraSideAngleStrength) * .3f);


        /*
        _virtualCameraComposer.m_TrackedObjectOffset =
            new Vector3(_horCameraOffset * defaultCameraTrackingHorInfluence + _cameraSideAngleStrength,
                _vertCameraOffset, 0);
        */
    }

    public void ToggleMap(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _mapOpen = !_mapOpen;
            OnMapToggle?.Invoke(_mapOpen);
        }
    }

    private void Update()
    {
        CalculateCameraOffset();
    }

    private void LateUpdate()
    {
        UpdatePlayerFollower();
    }
}