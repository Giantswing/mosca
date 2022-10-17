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

    public static UnityAction<bool> OnMapToggle;

    private bool _mapOpen = false;


    //CAMERA OFFSETS ////////
    private float _horCameraOffset;
    private float _horCameraOffsetTo;
    private float _vertCameraOffset;
    private float _vertCameraOffsetTo;
    private float _zoomCameraOffset;
    private float _zoomCameraOffsetTo;
    [HideInInspector] public float closeUpOffset = 0;
    [HideInInspector] public float closeUpOffsetTo = 0;

    /***********************************/

    private void Start()
    {
        virtualCamera = GameObject.Find("VirtualCamera")
            .GetComponent<CinemachineVirtualCamera>();
        _virtualCameraTransposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        _virtualCameraComposer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
    }

    private void CalculateCameraOffset()
    {
        _horCameraOffsetTo = pM.hSpeed + 3f * pM.isFacingRight;
        _horCameraOffset += (_horCameraOffsetTo - _horCameraOffset) * Time.deltaTime * 0.5f;

        _vertCameraOffsetTo = pM.vSpeed * 2.5f;
        _vertCameraOffset += (_vertCameraOffsetTo - _vertCameraOffset) * Time.deltaTime * 2f;


        _zoomCameraOffsetTo = pM.inputDirection.magnitude * 2.5f;
        _zoomCameraOffset += (_zoomCameraOffsetTo - _zoomCameraOffset) * Time.deltaTime * 0.7f;

        closeUpOffset += (closeUpOffsetTo - closeUpOffset) * Time.deltaTime * 1f;

        if (closeUpOffsetTo > 0)
            closeUpOffsetTo -= Time.deltaTime * 2f;

        _virtualCameraTransposer.m_FollowOffset =
            new Vector3(_horCameraOffset, _vertCameraOffset, defaultCameraZOffset - _zoomCameraOffset);

        _virtualCameraComposer.m_TrackedObjectOffset =
            new Vector3(_horCameraOffset * defaultCameraTrackingHorInfluence, _vertCameraOffset, 0);
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
}