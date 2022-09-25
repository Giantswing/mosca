using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerCamera pC;

    [HideInInspector] public float frozen = 0;

    private float _modelRotation = 0;
    private const float TimeToSwitch = .7f;
    [HideInInspector] public int isFacingRight = 1;
    private float _timeBackwards;

    private static readonly int FlyAnimSpeedH = Animator.StringToHash("FlyAnimSpeedH");
    private static readonly int FlyAnimSpeedV = Animator.StringToHash("FlyAnimSpeedV");


    [SerializeField] private GameObject my3DModel;

    public Animator flyAnimator;
    private readonly float acceleration = .035f;
    private readonly float speedMultiplier = 7f;


    [HideInInspector] public Vector2 inputDirection;
    private Vector2 _inputDirectionTo;
    [HideInInspector] public float hSpeed;
    [HideInInspector] public float vSpeed;

    private Rigidbody _myRigidbody;


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
        _myRigidbody = GetComponent<Rigidbody>();
        _inputDirectionTo = Vector2.zero;
        flyAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        frozen -= Time.deltaTime;

        if (_timerToDoubleDash > 0)
            _timerToDoubleDash -= Time.deltaTime;
        else
            _timerToDoubleDash = 0;

        if (_speedBoost > 1) _speedBoost -= Time.deltaTime;

        hSpeed += (_inputDirectionTo.x - hSpeed) * acceleration * Time.deltaTime * 60f;
        vSpeed += (_inputDirectionTo.y - vSpeed) * acceleration * Time.deltaTime * 60f;
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


            if ((_timeBackwards > TimeToSwitch || (isDashing && isFacingRight != 1 && _inputDirectionTo.x > 0)) &&
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

            if ((_timeBackwards > TimeToSwitch || (isDashing && isFacingRight == 1 && _inputDirectionTo.x < 0)) &&
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
            _myRigidbody.velocity = new Vector3(inputDirection.x * speedMultiplier * _speedBoost,
                inputDirection.y * speedMultiplier * _speedBoost, 0);
    }


    /* INPUTS *********************************************************/

    public void Move(InputAction.CallbackContext context)
    {
        _inputDirectionTo = context.ReadValue<Vector2>();
    }


    public void Dash(InputAction.CallbackContext context)
    {
        if (!isDashing && _timerToDoubleDash <= 0)
            flyAnimator.SetBool(IsDashing, true);
        else if (_timerToDoubleDash > 0)
            flyAnimator.SetBool(IsDoubleDashing, true);
    }

    /******************************************************************/


    public void StartDashBoost()
    {
        pC.closeUpOffsetTo = 1f;
        pC.closeUpOffset = 0;
        _speedBoost = _speedBoostMax;
        inputDirection = _inputDirectionTo;
        hSpeed = inputDirection.x;
        vSpeed = inputDirection.y;
        isDashing = true;
    }

    public void StartDoubleDash()
    {
        StartDashBoost();
        _doubleDash = true;
        _speedBoost = _speedBoostMax * 1.5f;
    }

    public void StopDashBoost()
    {
        isDashing = false;
        _speedBoost = 1;
        flyAnimator.SetBool(IsDashing, false);
    }

    public void StopDoubleDash()
    {
        StopDashBoost();
        flyAnimator.SetBool(IsDoubleDashing, false);
        _doubleDash = false;
    }

    public void AllowDoubleDash()
    {
        _timerToDoubleDash = _timerToDoubleDashMax;
    }

    /******************************************************************/
}