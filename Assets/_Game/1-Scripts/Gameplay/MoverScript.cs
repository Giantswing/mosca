using System;
using System.Collections;
using System.Collections.Generic;
using AmplifyShaderEditor;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class MovePoint
{
    public Vector3 offset;
    public Vector3 rotation;
    public float waitTime;
}

public class MoverScript : MonoBehaviour
{
    [SerializeField] private bool pingPong = false;
    [SerializeField] private bool smoothMove = true;
    private Ease ease;
    private int _direction = 1;


    public List<MovePoint> MovePoints = new();
    private int _currentMovePoint = 0;
    private Vector3 _startPosition;
    private WaitForSeconds[] _waitTimes;
    private bool _changedThisFrame = false;


    [SerializeField] private float moveSpeed = 1f;

    private void Start()
    {
        if (smoothMove)
            ease = Ease.InOutQuad;
        else
            ease = Ease.Linear;

        _startPosition = transform.position;
        _waitTimes = new WaitForSeconds[MovePoints.Count];

        for (var i = 0; i < MovePoints.Count; i++) _waitTimes[i] = new WaitForSeconds(MovePoints[i].waitTime);

        if (MovePoints.Count > 0)
        {
            transform.position = _startPosition + MovePoints[0].offset;
            IterateMovePoint();
            Move();
        }
    }

    private void IterateMovePoint()
    {
        if (!pingPong)
        {
            if (_currentMovePoint < MovePoints.Count - 1)
                _currentMovePoint++;
            else
                _currentMovePoint = 0;
        }
        else
        {
            _changedThisFrame = false;
            if (_direction == 1)
            {
                if (_currentMovePoint < MovePoints.Count - 1)
                {
                    _currentMovePoint++;
                }
                else
                {
                    _currentMovePoint = MovePoints.Count - 2;
                    _direction = -1;
                    _changedThisFrame = true;
                }
            }

            if (_direction == -1 && _changedThisFrame == false)
            {
                if (_currentMovePoint > 0)
                {
                    _currentMovePoint--;
                }
                else
                {
                    _currentMovePoint = 1;
                    _direction = 1;
                    _changedThisFrame = true;
                }
            }
        }
    }

    private void Move()
    {
        var distanceToNextPoint =
            Vector3.Distance(transform.position, _startPosition + MovePoints[_currentMovePoint].offset);

        transform.DOMove(_startPosition + MovePoints[_currentMovePoint].offset,
                distanceToNextPoint / moveSpeed * .5f).SetEase(ease).onComplete +=
            () => { StartCoroutine(WaitMove()); };
    }

    private void OnDisable()
    {
        transform.DOKill();
    }

    private IEnumerator WaitMove()
    {
        yield return _waitTimes[_currentMovePoint];
        transform.DORotate(MovePoints[_currentMovePoint].rotation, .5f).SetEase(Ease.InOutQuad);
        IterateMovePoint();
        Move();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            _startPosition = transform.position;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, 0.7f);

        Gizmos.color = Color.white;
        for (var i = 0; i < MovePoints.Count; i++)
        {
            var point = _startPosition + MovePoints[i].offset;

            Gizmos.DrawSphere(point, .2f);
            Gizmos.DrawLine(point, point + Quaternion.Euler(MovePoints[i].rotation) * Vector3.right);
        }
    }
}