using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Utilities;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerCamera pC;
    [SerializeField] private PlayerSoundManager pS;

    [HideInInspector] public float frozen = 0;

    public static PlayerMovement instance;
    private Transform _transform;

    private float _modelRotation = 0;
    private const float TimeToSwitch = .35f;
    [HideInInspector] public int isFacingRight = 1;
    private float _timeBackwards;

    private static readonly int FlyAnimSpeedH = Animator.StringToHash("FlyAnimSpeedH");
    private static readonly int FlyAnimSpeedV = Animator.StringToHash("FlyAnimSpeedV");


    public GameObject my3DModel;

    public Animator flyAnimator;
    private readonly float acceleration = .1f;
    private readonly float speedMultiplier = 7f;

    [HideInInspector] public float lastBumpTime = 0;


    [HideInInspector] public Vector2 inputDirection;
    [HideInInspector] public Vector2 inputDirectionTo;
    [HideInInspector] public float hSpeed;
    [HideInInspector] public float vSpeed;

    [SerializeField] private Rigidbody _myRigidbody;

    private bool imDisabled = false;

    private PlayerInput _playerInput;

    [SerializeField] private Collider dashCollider;

    private Vector3 _windForce;
    public Vector3 windForceTo;


    // MOBILE INPUT /////////////////////////
    public static UnityAction<Vector2, Vector2> onTouchInput;
    private Vector2 _touchStartingPos;
    private Vector2 _touchCurrentPos;

    [Range(0.01f, 0.2f)] [SerializeField] private float maxTouchDistanceScreen = 0.1f;
    [Range(0, 50f)] [SerializeField] private float cornerPadding = 10f;
    [Range(0.1f, 0.3f)] [SerializeField] private float maxHorTouchDistance = .2f;

    private float _touchDeltaX;
    private float _touchDeltaY;
    private float maxTouchDistance;
    private float _touchMagnitude;
    [SerializeField] private float touchMultiplier = 1f;
    private float _touchFixSpeed = .5f;

    //DASH /////////////////////

    private float _timerToDoubleDash = 0.5f;
    private float _timerToDoubleDashMax = .35f;
    private float _speedBoost = 1;
    private float _speedBoostMax = 2f;
    [HideInInspector] public bool isDashing = false;
    private bool _doubleDash = false;
    private static readonly int IsDashing = Animator.StringToHash("IsDashing");
    private static readonly int IsDoubleDashing = Animator.StringToHash("IsDoubleDashing");

    //////////////////////////


    // Start is called before the first frame update
    private void Start()
    {
        instance = this;
        //_myRigidbody = GetComponent<Rigidbody>();
        inputDirectionTo = Vector2.zero;
        flyAnimator = GetComponentInChildren<Animator>();
        _transform = transform;
    }

    private void OnEnable()
    {
        LevelManager.StartLevelTransition += DisableMovement;
    }

    private void OnDisable()
    {
        LevelManager.StartLevelTransition -= DisableMovement;
    }

    private void DisableMovement(int levelTransitionState, SceneField levelToLoad)
    {
        GetComponent<PlayerInput>().enabled = false;
    }

    private void EnableMovement()
    {
        GetComponent<PlayerInput>().enabled = true;
    }

    public static Transform ReturnPlayerTransform()
    {
        return instance._transform;
    }

    public Vector3 ReturnVelocity()
    {
        return _myRigidbody.velocity;
    }

    // Update is called once per frame
    private void Update()
    {
        if (imDisabled) return;

        lastBumpTime -= Time.deltaTime;

        _windForce = Vector3.Lerp(_windForce, windForceTo, Time.deltaTime * 5);
        windForceTo = Vector3.Lerp(windForceTo, Vector3.zero, Time.deltaTime * 5);

        frozen -= Time.deltaTime;

        if (_timerToDoubleDash > 0)
            _timerToDoubleDash -= Time.deltaTime;
        else
            _timerToDoubleDash = 0;

        if (_speedBoost > 1) _speedBoost -= Time.deltaTime;

        hSpeed += (inputDirectionTo.x - hSpeed) * acceleration * Time.deltaTime * 60f;
        vSpeed += (inputDirectionTo.y - vSpeed) * acceleration * Time.deltaTime * 60f;
        inputDirection = new Vector2(hSpeed, vSpeed);


        flyAnimator.SetFloat(FlyAnimSpeedH, inputDirection.x * isFacingRight);
        flyAnimator.SetFloat(FlyAnimSpeedV, inputDirection.y);


        /*

        if (_inputDirectionTo == Vector2.zero)
        {
            _hSpeed -= _hSpeed * _deceleration * Time.deltaTime;
            _vSpeed -= _vSpeed * _deceleration * Time.deltaTime;
        }
        
        */

        if (hSpeed >= .5f / 2 || isDashing)
        {
            if (isFacingRight != 1)
                _timeBackwards += Time.deltaTime;


            if ((_timeBackwards > TimeToSwitch || (isDashing && isFacingRight != 1 && inputDirectionTo.x > 0)) &&
                !_doubleDash)
            {
                my3DModel.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
                _timeBackwards = 0;
                isFacingRight = 1;
            }
        }


        if (hSpeed <= -.5f / 2 || isDashing)
        {
            if (isFacingRight == 1)
                _timeBackwards += Time.deltaTime;

            if ((_timeBackwards > TimeToSwitch || (isDashing && isFacingRight == 1 && inputDirectionTo.x < 0)) &&
                !_doubleDash)
            {
                my3DModel.transform.DOLocalRotate(new Vector3(0, 180, 0), 0.5f);
                _timeBackwards = 0;
                isFacingRight = -1;
            }
        }

        if (Math.Abs(hSpeed) < 0.2f) _timeBackwards = 0;


        if (isDashing)
        {
            if (isFacingRight == 1)
                _modelRotation = Mathf.LerpAngle(_modelRotation, Mathf.Atan2(vSpeed, hSpeed) * Mathf.Rad2Deg, .9f);
            else
                _modelRotation = Mathf.LerpAngle(_modelRotation, Mathf.Atan2(vSpeed, hSpeed) * Mathf.Rad2Deg - 180f,
                    .9f);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, _modelRotation),
                Time.deltaTime * 15f);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity,
                Time.deltaTime * 4f);
        }
    }

    private void LateUpdate()
    {
        if (frozen <= 0)
            _myRigidbody.velocity = new Vector3(inputDirection.x * speedMultiplier * _speedBoost + _windForce.x,
                inputDirection.y * speedMultiplier * _speedBoost + _windForce.y, 0);
    }


    /* INPUTS *********************************************************/

    public void Move(InputAction.CallbackContext context)
    {
        inputDirectionTo = context.ReadValue<Vector2>();
    }


    public void Dash(InputAction.CallbackContext context)
    {
        if (imDisabled) return;

        if (!isDashing && _timerToDoubleDash <= 0)
            flyAnimator.SetBool(IsDashing, true);
        else if (_timerToDoubleDash > 0)
            flyAnimator.SetBool(IsDoubleDashing, true);
    }

    public void TouchInput(InputAction.CallbackContext context)
    {
        _touchMagnitude = 0;
        var width = Screen.width;
        maxTouchDistance = width * maxTouchDistanceScreen;
        var touchContext = context.ReadValue<TouchState>();

        if (touchContext.position.x > width * .45f) return;

        if (touchContext.phaseId == (int)TouchPhase.Began)
            _touchStartingPos = touchContext.position;

        if (context.performed)
        {
            if (_touchStartingPos == Vector2.zero)
                _touchStartingPos = _touchCurrentPos;

            _touchCurrentPos = touchContext.position;

            _touchMagnitude = Mathf.Lerp(0, touchMultiplier,
                (_touchCurrentPos - _touchStartingPos).magnitude / maxTouchDistance);
            _touchMagnitude = Mathf.Clamp01(_touchMagnitude);

            var finalTouchMovement = (_touchCurrentPos - _touchStartingPos).normalized * _touchMagnitude;

            inputDirectionTo = finalTouchMovement;

            if (Vector2.Distance(_touchStartingPos, _touchCurrentPos) > maxTouchDistance * 1.2f)
            {
                if (_touchCurrentPos.x > _touchStartingPos.x && _touchStartingPos.x > width * maxHorTouchDistance)
                    _touchDeltaX = 0;
                else
                    _touchDeltaX = (_touchCurrentPos - _touchStartingPos).normalized.x * _touchFixSpeed;

                _touchDeltaY = (_touchCurrentPos - _touchStartingPos).normalized.y * _touchFixSpeed;

                _touchStartingPos = new Vector2(_touchStartingPos.x + _touchDeltaX, _touchStartingPos.y + _touchDeltaY);
            }
        }

        if (_touchStartingPos.y < maxTouchDistance + cornerPadding)
            _touchStartingPos = Vector2.MoveTowards(_touchStartingPos, _touchStartingPos + Vector2.up * 100f, 4f);

        if (_touchStartingPos.x < maxTouchDistance + cornerPadding)
            _touchStartingPos = Vector2.MoveTowards(_touchStartingPos, _touchStartingPos + Vector2.right * 100f, 4f);

        if (touchContext.phaseId == (int)TouchPhase.Ended)
        {
            inputDirectionTo = Vector2.zero;
            _touchStartingPos = Vector2.zero;
        }

        onTouchInput?.Invoke(_touchStartingPos, _touchCurrentPos);

        //print(_touchStartingPos + " " + _touchCurrentPos);
    }

    /******************************************************************/


    public void StartDashBoost()
    {
        pC.closeUpOffsetTo = 1f;
        pC.closeUpOffset = 0;
        _speedBoost = _speedBoostMax;
        inputDirection = inputDirectionTo;
        hSpeed = inputDirection.x;
        vSpeed = inputDirection.y;
        isDashing = true;
        pS.PlayDashSound();

        dashCollider.enabled = true;
    }

    public void StartDoubleDash()
    {
        pC.closeUpOffsetTo = 1f;
        pC.closeUpOffset = 0;
        _speedBoost = _speedBoostMax * 1.5f;
        inputDirection = inputDirectionTo;
        hSpeed = inputDirection.x;
        vSpeed = inputDirection.y;
        isDashing = true;

        dashCollider.enabled = true;
        _doubleDash = true;
    }

    public void StopDashBoost()
    {
        isDashing = false;
        _speedBoost = 1;
        flyAnimator.SetBool(IsDashing, false);

        dashCollider.enabled = true;
    }

    public void StopDoubleDash()
    {
        StopDashBoost();
        flyAnimator.SetBool(IsDoubleDashing, false);
        _doubleDash = false;

        dashCollider.enabled = true;
    }

    public void AllowDoubleDash()
    {
        _timerToDoubleDash = _timerToDoubleDashMax;
    }

    /******************************************************************/

    public void DisablePlayer()
    {
        hSpeed = 0;
        vSpeed = 0;
        inputDirection = Vector2.zero;
        inputDirectionTo = Vector2.zero;
        _myRigidbody.velocity = Vector3.zero;
        imDisabled = true;
    }
}