using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Random = UnityEngine.Random;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class PlayerMovement : MonoBehaviour
{
    private const float TimeToSwitch = .35f;

    public static PlayerMovement instance;

    private static readonly int FlyAnimSpeedH = Animator.StringToHash("FlyAnimSpeedH");
    private static readonly int FlyAnimSpeedV = Animator.StringToHash("FlyAnimSpeedV");


    // MOBILE INPUT /////////////////////////
    public static UnityAction<Vector2, Vector2> onTouchInput;
    private static readonly int IsDashing = Animator.StringToHash("IsDashing");
    private static readonly int IsDoubleDashing = Animator.StringToHash("IsDoubleDashing");
    private static readonly int IsDodging = Animator.StringToHash("IsDodging");
    [SerializeField] private PlayerCamera pC;
    [SerializeField] private PlayerSoundManager pS;
    [SerializeField] private PlayerInteractionHandler pI;

    [HideInInspector] public float frozen;
    [HideInInspector] public int isFacingRight = 1;

    [SerializeField] private float maxDistanceToCheckpoint = 10f;


    public GameObject my3DModel;

    public Animator flyAnimator;

    [HideInInspector] public float lastBumpTime;


    [HideInInspector] public Vector2 inputDirection;
    [HideInInspector] public Vector2 inputDirectionTo;
    [HideInInspector] public float hSpeed;
    [HideInInspector] public float vSpeed;
    [HideInInspector] public float zDepthTo;
    [HideInInspector] public float zDepth;
    public float zDepthOffset = 1.6f;
    public LayerMask backgroundLayer;

    [SerializeField] private Rigidbody _myRigidbody;

    [SerializeField] private Collider dashCollider;
    [SerializeField] private STATS stats;
    public Vector3 windForceTo;
    public float currentlyInWind;

    [Range(0.01f, 0.2f)] [SerializeField] private float maxTouchDistanceScreen = 0.1f;
    [Range(0, 50f)] [SerializeField] private float cornerPadding = 10f;
    [Range(0.1f, 0.3f)] [SerializeField] private float maxHorTouchDistance = .2f;
    [SerializeField] private float touchMultiplier = 1f;
    [HideInInspector] public bool isDashing;


    ////////////////////////

    // DODGE ///////////////////////

    [SerializeField] private Color dodgeColor;
    [HideInInspector] public bool isDodging;
    private readonly WaitForSeconds _dodgeDuration = new(0.3f);
    private readonly float _speedBoostMax = 2f;
    private readonly float _timerToDoubleDashMax = .35f;
    private readonly float _touchFixSpeed = .5f;
    private readonly float acceleration = .1f;
    private readonly float speedMultiplier = 7f;
    private bool _canDodge = true;
    private bool _checkForCheckpoint = true;
    private CheckpointScript[] _checkpoints;
    private int _currentCheckpoint;
    private Vector3 _dodgeDirection;
    private bool _doubleDash;


    private float _modelRotation;

    private PlayerInput _playerInput;
    private float _speedBoost = 1;
    private float _startingZDepth;
    private float _timeBackwards;

    //DASH /////////////////////

    private float _timerToDoubleDash = 0.5f;
    private float _timeStandingStill;
    private Vector2 _touchCurrentPos;

    private float _touchDeltaX;
    private float _touchDeltaY;
    private float _touchMagnitude;
    private Vector2 _touchStartingPos;
    private Transform _transform;

    private Vector3 _windForce;
    private float _zRot;
    private float _zRotTo;

    private bool imDisabled;
    private float maxTouchDistance;

    ////////////////////////


    // Start is called before the first frame update
    private void Start()
    {
        instance = this;
        //_myRigidbody = GetComponent<Rigidbody>();
        inputDirectionTo = Vector2.zero;
        flyAnimator = GetComponentInChildren<Animator>();
        _transform = transform;
        _startingZDepth = _transform.position.z;
        print(_startingZDepth);
        zDepth = _startingZDepth;
        zDepthTo = _startingZDepth;
    }

    // Update is called once per frame
    private void Update()
    {
        if (imDisabled) return;

        lastBumpTime -= Time.deltaTime;

        if (stats.ST_Invincibility)
        {
            _windForce = Vector3.Lerp(_windForce, Vector3.zero, Time.deltaTime * 10f);
            windForceTo = Vector3.Lerp(windForceTo, Vector3.zero, Time.deltaTime * 10f);
        }
        else
        {
            _windForce = Vector3.Lerp(_windForce, windForceTo, Time.deltaTime * 10f);
            currentlyInWind += Time.deltaTime;

            if (currentlyInWind > 0.05f)
                windForceTo = Vector3.Lerp(windForceTo, Vector3.zero, Time.deltaTime * 10f);
            else
                windForceTo = Vector3.Lerp(windForceTo, Vector3.zero, Time.deltaTime * 4f);
        }

        frozen -= Time.deltaTime;

        if (_timerToDoubleDash > 0)
            _timerToDoubleDash -= Time.deltaTime;
        else
            _timerToDoubleDash = 0;

        if (_speedBoost > 1) _speedBoost -= Time.deltaTime;

        hSpeed += (inputDirectionTo.x - hSpeed) * acceleration * Time.deltaTime * 50f;
        vSpeed += (inputDirectionTo.y - vSpeed) * acceleration * Time.deltaTime * 50f;
        inputDirection = new Vector2(hSpeed, vSpeed);

        flyAnimator.SetFloat(FlyAnimSpeedH, inputDirection.x * isFacingRight);
        flyAnimator.SetFloat(FlyAnimSpeedV, inputDirection.y);

        CheckIfPlayerShouldFlip();

        CheckZDepth();
        UpdatePlayerRotation();
    }

    private void LateUpdate()
    {
        if (frozen <= 0)
            _myRigidbody.velocity = new Vector3(inputDirection.x * speedMultiplier * _speedBoost + _windForce.x,
                inputDirection.y * speedMultiplier * _speedBoost + _windForce.y, 0);
    }

    public void DisableMovement(int levelTransitionState, SceneField levelToLoad)
    {
        GetComponent<PlayerInput>().enabled = false;
    }

    public void EnableMovement()
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

    public bool IncreaseCheckpoint(int checkpointIndex)
    {
        if (checkpointIndex == _currentCheckpoint)
        {
            _currentCheckpoint = checkpointIndex + 1;
            return true;
        }

        return false;
    }

    private void CheckZDepth()
    {
        var foundWall = false;
        //cast a ray from the player to the ground
        if (Physics.Raycast(transform.position, transform.forward, out var hit, 6f,
                backgroundLayer))
        {
            zDepthTo = hit.distance;
            foundWall = true;
        }
        else
        {
            zDepthTo = _startingZDepth;
        }


        zDepth = Mathf.Lerp(zDepth, zDepthTo, Time.deltaTime * 50f);
        var position = transform.position;
        position = transform.position + transform.forward * (zDepth - zDepthOffset);
        transform.position = position;


        if (foundWall)
        {
            var rotationToLook = Quaternion.LookRotation(-hit.normal, Vector3.up);
            _zRotTo = rotationToLook.eulerAngles.y;
            _zRot = Mathf.Lerp(_zRot, _zRotTo, Time.deltaTime * 30f);
        }
    }

    //draw ray gizmos
    /*
    private void OnDrawGizmos()
    {   
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.forward*5);
    }
    */

    private void UpdatePlayerRotation()
    {
        //var rot = (zDepth - zDepthOffset) * 15f * -isFacingRight;
        var rot = _zRotTo;
        //var rot = 0;
        if (isDashing)
        {
            if (isFacingRight == 1)
                _modelRotation = Mathf.LerpAngle(_modelRotation, Mathf.Atan2(vSpeed, hSpeed) * Mathf.Rad2Deg, .9f);
            else
                _modelRotation = Mathf.LerpAngle(_modelRotation, Mathf.Atan2(vSpeed, hSpeed) * Mathf.Rad2Deg - 180f,
                    .9f);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, rot, _modelRotation),
                Time.deltaTime * 15f);
        }
        else if (isDodging)
        {
            _modelRotation = Mathf.LerpAngle(_modelRotation,
                Mathf.Atan2(_dodgeDirection.x, _dodgeDirection.y) * Mathf.Rad2Deg, .9f);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, rot, _modelRotation),
                Time.deltaTime * 15f);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, rot, 0),
                Time.deltaTime * 4f);
        }
    }

    private void CheckIfPlayerShouldFlip()
    {
        if (hSpeed >= .5f / 2 || isDashing)
        {
            if (isFacingRight != 1)
                _timeBackwards += Time.deltaTime;


            if ((_timeBackwards > TimeToSwitch || (isDashing && isFacingRight != 1 && inputDirectionTo.x > 0)) &&
                !_doubleDash)
                FlipPlayer(1, 0.5f);
        }

        if (hSpeed <= -.5f / 2 || isDashing)
        {
            if (isFacingRight == 1)
                _timeBackwards += Time.deltaTime;

            if ((_timeBackwards > TimeToSwitch || (isDashing && isFacingRight == 1 && inputDirectionTo.x < 0)) &&
                !_doubleDash)
                FlipPlayer(-1, 0.5f);
        }

        if (Math.Abs(hSpeed) < 0.2f && !isDodging) _timeBackwards = 0;

        if (inputDirectionTo.magnitude < 0.1f)
        {
            _timeStandingStill += Time.deltaTime;
        }
        else
        {
            _timeStandingStill = 0;
            _checkForCheckpoint = true;
        }

        if (_timeStandingStill > 1f && _checkForCheckpoint)
        {
            _checkpoints = LevelManager.GetCheckpoints().ToArray();
            CheckCheckpoints();
        }
    }

    private void CheckCheckpoints()
    {
        _checkForCheckpoint = false;
        _timeStandingStill = 0;
        if (_checkpoints.Length == 0) return;

        if (_checkpoints[_currentCheckpoint].isActivated ||
            _checkpoints[_currentCheckpoint].pauseCheckpoint ||
            _currentCheckpoint > _checkpoints.Length) return;

        if (Vector3.Distance(_transform.position, _checkpoints[_currentCheckpoint].transform.position) <
            maxDistanceToCheckpoint)
        {
            if (_checkpoints[_currentCheckpoint].transform.position.x > _transform.position.x && isFacingRight == -1)
                FlipPlayer(1, 0.5f);
            else if (_checkpoints[_currentCheckpoint].transform.position.x < _transform.position.x &&
                     isFacingRight == 1)
                FlipPlayer(-1, 0.5f);
        }
    }

    private void FlipPlayer(int direction, float speed)
    {
        if (direction == 1)
        {
            my3DModel.transform.DOLocalRotate(new Vector3(0, 0, 0), speed);
            _timeBackwards = 0;
            isFacingRight = 1;
        }
        else
        {
            my3DModel.transform.DOLocalRotate(new Vector3(0, 180, 0), speed);
            _timeBackwards = 0;
            isFacingRight = -1;
        }
    }


    /* INPUTS *********************************************************/

    public void Move(InputAction.CallbackContext context)
    {
        inputDirectionTo = context.ReadValue<Vector2>();
    }


    public void Dash(InputAction.CallbackContext context)
    {
        if (imDisabled) return;

        if (inputDirectionTo == Vector2.zero && isDodging == false && _canDodge)
        {
            flyAnimator.SetBool(IsDodging, true);
            StartCoroutine(DodgeCoroutine());
        }
        else if (inputDirectionTo != Vector2.zero)
        {
            if (!isDashing && _timerToDoubleDash <= 0)
                flyAnimator.SetBool(IsDashing, true);
            else if (_timerToDoubleDash > 0)
                flyAnimator.SetBool(IsDoubleDashing, true);
        }
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

    private IEnumerator DodgeCoroutine()
    {
        _canDodge = false;

        /*
        my3DModel.transform.DOLocalRotate(new Vector3(0, 360, 0), 1f, RotateMode.FastBeyond360);
        */

        pS.PlayDodgeSound();
        _dodgeDirection = Random.insideUnitCircle.normalized;
        var finalDodgePos = transform.position + _dodgeDirection * 1.3f;
        var originalPos = transform.position;

        var hasRotated = false;

        if (_dodgeDirection.x < 0 && isFacingRight == 1)
        {
            my3DModel.transform.DOLocalRotate(new Vector3(0, -180, 0), 0.3f);
            hasRotated = true;
        }
        else if (_dodgeDirection.x > 0 && isFacingRight == -1)
        {
            my3DModel.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.3f);
            hasRotated = true;
        }

        transform.DOMove(finalDodgePos, 0.2f);
        isDodging = true;
        pI.GlowPlayer(dodgeColor);
        pI.MakeInvincible(true);
        EffectHandler.SpawnFX(2, transform.position, Vector3.zero, Vector3.zero, 0);
        yield return _dodgeDuration;
        flyAnimator.SetBool(IsDodging, false);
        isDodging = false;

        if (hasRotated)
        {
            if (_dodgeDirection.x < 0)
                my3DModel.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.3f);
            else if (_dodgeDirection.x > 0 && isFacingRight == -1)
                my3DModel.transform.DOLocalRotate(new Vector3(0, 180, 0), 0.3f);
        }

        transform.DOMove(originalPos, 0.2f).onComplete += () =>
        {
            pI.MakeInvincible(false);
            _canDodge = true;
        };
    }

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
/*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistanceToCheckpoint);
    }
    */
}