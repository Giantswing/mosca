using System;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class FlyMovement : MonoBehaviour
{
    private const float TimeToSwitch = .7f;
    private int _isFacingRight = 1;
    private float _timeBackwards;

    private static readonly int FlyAnimSpeedH = Animator.StringToHash("FlyAnimSpeedH");
    private static readonly int FlyAnimSpeedV = Animator.StringToHash("FlyAnimSpeedV");


    [SerializeField] private GameObject my3DModel;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private CinemachineTransposer _virtualCameraTransposer;

    public Animator flyAnimator;
    private readonly float _deceleration = 5f;
    private readonly float acceleration = 2f;
    private readonly float maxMovSpeed = 1f;
    private readonly float speedMultiplier = 10f;


    private Vector2 _inputDirection;
    private Vector2 _movVector;
    private float _hSpeed;
    private float _vSpeed;

    private Rigidbody _myRigidbody;


    //CAMERA OFFSETS ////////
    private float _horCameraOffset;
    private float _horCameraOffsetTo;
    private float _vertCameraOffset;
    private float _vertCameraOffsetTo;
    private float _zoomCameraOffset;
    private float _zoomCameraOffsetTo;
    private float _closeUpOffset = 0;
    private float _closeUpOffsetTo = 0;

    /***********************************/


    //DASH /////////////////////

    private float _dashCooldown = 0;
    [SerializeField] private float dashCooldownMax = 1f;
    private float _speedBoost = 1;
    private float _speedBoostMax = 2f;
    private bool _isDashing = false;
    private static readonly int IsDashing = Animator.StringToHash("IsDashing");

    //////////////////////////


    // Start is called before the first frame update
    private void Start()
    {
        _myRigidbody = GetComponent<Rigidbody>();
        _movVector = Vector2.zero;
        _inputDirection = Vector2.zero;
        flyAnimator = GetComponentInChildren<Animator>();
        virtualCamera = GameObject.Find("VirtualCamera")
            .GetComponent<CinemachineVirtualCamera>();
        _virtualCameraTransposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
    }

    // Update is called once per frame
    private void Update()
    {
        _dashCooldown -= Time.deltaTime;

        if (_speedBoost > 1) _speedBoost -= Time.deltaTime;

        _hSpeed += _inputDirection.x * acceleration * Time.deltaTime;
        _hSpeed = Mathf.Clamp(_hSpeed, -maxMovSpeed, maxMovSpeed);


        _vSpeed += _inputDirection.y * acceleration * Time.deltaTime;
        _vSpeed = Mathf.Clamp(_vSpeed, -maxMovSpeed, maxMovSpeed);
        _movVector = new Vector2(_hSpeed, _vSpeed);

        _myRigidbody.velocity = new Vector3(_movVector.x * speedMultiplier * _speedBoost,
            _movVector.y * speedMultiplier * _speedBoost, 0);


        flyAnimator.SetFloat(FlyAnimSpeedH, _movVector.x * _isFacingRight);
        flyAnimator.SetFloat(FlyAnimSpeedV, _movVector.y);

        CalculateCameraOffset();

        if (_inputDirection == Vector2.zero)
        {
            _hSpeed -= _hSpeed * _deceleration * Time.deltaTime;
            _vSpeed -= _vSpeed * _deceleration * Time.deltaTime;
        }

        if (_hSpeed >= maxMovSpeed / 2 || _isDashing)
        {
            if (_isFacingRight != 1)
                _timeBackwards += Time.deltaTime;


            if (_timeBackwards > TimeToSwitch || (_isDashing && _isFacingRight != 1 && _inputDirection.x > 0))
            {
                my3DModel.transform.DORotate(new Vector3(0, 90, 0), 0.5f);
                _timeBackwards = 0;
                _isFacingRight = 1;
            }
        }


        if (_hSpeed <= -maxMovSpeed / 2 || _isDashing)
        {
            if (_isFacingRight == 1)
                _timeBackwards += Time.deltaTime;

            if (_timeBackwards > TimeToSwitch || (_isDashing && _isFacingRight == 1 && _inputDirection.x < 0))
            {
                my3DModel.transform.DORotate(new Vector3(0, 270, 0), 0.5f);
                _timeBackwards = 0;
                _isFacingRight = -1;
            }
        }

        if (Math.Abs(_hSpeed) < 0.2f) _timeBackwards = 0;
    }

    /* COLLISIONS *********************************************************/
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Breakable") && _isDashing)
        {
            FreezeFrameScript.FreezeFrames(0.2f);
            _closeUpOffset = .35f;
            _closeUpOffsetTo = 1f;
            Destroy(collision.gameObject);
        }
        else
        {
            _movVector = Vector2.Reflect(_movVector, collision.contacts[0].normal);
            _hSpeed = _movVector.x * .5f;
            _vSpeed = _movVector.y * .5f;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
            GameManagerScript.coinList.Remove(collision.gameObject);
            GameManagerScript.AddScore(1);
        }
        else if (collision.gameObject.CompareTag("Meta"))
        {
            if (collision.GetComponent<MetaScript>().isOpen)
            {
                Destroy(collision.gameObject);
                GameManagerScript.WinGame();
            }
        }
    }

    /*********************************************************/


    /* INPUTS *********************************************************/

    public void Move(InputAction.CallbackContext context)
    {
        _inputDirection = context.ReadValue<Vector2>();
    }


    public void Dash(InputAction.CallbackContext context)
    {
        if (_dashCooldown <= 0) flyAnimator.SetBool(IsDashing, true);
    }

    /******************************************************************/


    public void StopDashBoost()
    {
        _isDashing = false;
        _speedBoost = 1;
        flyAnimator.SetBool(IsDashing, false);
    }

    public void StartDashBoost()
    {
        _closeUpOffsetTo = 1f;
        _closeUpOffset = 0;

        _dashCooldown = dashCooldownMax;
        _speedBoost = _speedBoostMax;
        _isDashing = true;
    }


    /******************************************************************/

    private void CalculateCameraOffset()
    {
        _horCameraOffsetTo = _hSpeed + 3f * _isFacingRight;
        _horCameraOffset += (_horCameraOffsetTo - _horCameraOffset) * Time.deltaTime * 2f;

        _vertCameraOffsetTo = _vSpeed * 1.5f;
        _vertCameraOffset += (_vertCameraOffsetTo - _vertCameraOffset) * Time.deltaTime * 2f;

        _zoomCameraOffsetTo = _movVector.magnitude * 5f;
        _zoomCameraOffset += (_zoomCameraOffsetTo - _zoomCameraOffset) * Time.deltaTime * 0.7f;

        _virtualCameraTransposer.m_FollowOffset =
            new Vector3(_horCameraOffset, _vertCameraOffset, -8f - _movVector.magnitude + _closeUpOffset * 2.5f);

        _closeUpOffset += (_closeUpOffsetTo - _closeUpOffset) * Time.deltaTime * 3f;

        if (_closeUpOffsetTo > 0)
            _closeUpOffsetTo -= Time.deltaTime * 2f;
    }
}