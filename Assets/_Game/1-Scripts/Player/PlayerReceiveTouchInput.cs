using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.TouchPhase;

public class PlayerReceiveTouchInput : MonoBehaviour
{
    //public static UnityAction<Vector2, Vector2> onTouchInput;
    public Action<Vector2> OnMove;
    [Range(0.01f, 0.2f)] [SerializeField] private float maxTouchDistanceScreen = 0.1f;
    [Range(0, 50f)] [SerializeField] private float cornerPadding = 10f;
    [Range(0.1f, 0.3f)] [SerializeField] private float maxHorTouchDistance = .2f;
    [SerializeField] private float touchMultiplier = 1f;
    private readonly float _touchFixSpeed = .5f;
    private Vector2 _touchCurrentPos;

    private float _touchDeltaX;
    private float _touchDeltaY;
    private float _touchMagnitude;
    private Vector2 _touchStartingPos;
    private float maxTouchDistance;

    private Vector2 inputDirectionTo;

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


        OnMove?.Invoke(inputDirectionTo);
    }
}