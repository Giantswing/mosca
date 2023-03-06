using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public static UnityAction<bool> OnMapToggle;
    [SerializeField] private PlayerMovement pM;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [SerializeField] private float defaultCameraZOffset = -7f;

    [SerializeField] private float defaultCameraSideAngleStrength = .7f;

    [SerializeField] private Transform playerFollower;
    [SerializeField] private float cameraOffsetStrength = 1f;
    [HideInInspector] public float closeUpOffset;
    [HideInInspector] public float closeUpOffsetTo;

    [Header("Camera Zone")] [Space(5)] [SerializeField]
    private CameraZone currentCameraZone;

    private Vector2 _cameraInput;
    private Vector2 _cameraInputTo;

    private float _cameraSideAngleStrength;
    private float _cameraSideAngleStrengthTo;

    private Vector3 _cameraZoneOffset;
    private float _cameraZoneZoom;


    //CAMERA OFFSETS ////////
    private float _horCameraOffset;
    private float _horCameraOffsetTo;

    private bool _mapOpen;

    private float _timeAlive;
    private float _vertCameraOffset;
    private float _vertCameraOffsetTo;
    private CinemachineComposer _virtualCameraComposer;

    //[SerializeField] private float defaultCameraTrackingHorInfluence = 0.2f;
    private CinemachineTransposer _virtualCameraTransposer;
    private float _zoomCameraOffset;
    private float _zoomCameraOffsetTo;

    /***********************************/

    private void Start()
    {
        virtualCamera = GameObject.Find("VirtualCamera")
            .GetComponent<CinemachineVirtualCamera>();
        _virtualCameraTransposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        _virtualCameraComposer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
        _cameraSideAngleStrengthTo = 1;
        _cameraSideAngleStrength = 1;
        _cameraInputTo = Vector2.zero;
    }

    private void Update()
    {
        /*
        _timeAlive += Time.deltaTime;
        if (_timeAlive < 0.2f)
            _cameraInputTo = Vector2.zero;
            */
        CalculateCameraOffset();
    }

    private void LateUpdate()
    {
        UpdatePlayerFollower();
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
            transform.position +
            new Vector3(_horCameraOffset + _cameraInput.x, _vertCameraOffset + _cameraInput.y, 0);
    }

    private void CalculateCameraOffset()
    {
        _cameraInput = Vector2.Lerp(_cameraInput, _cameraInputTo, Time.deltaTime * 5f);

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

    public void MoveCamera(InputAction.CallbackContext context)
    {
        if (context.performed) _cameraInputTo = context.ReadValue<Vector2>() * cameraOffsetStrength;
    }
}