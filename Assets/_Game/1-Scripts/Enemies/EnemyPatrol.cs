using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class PatrolPoint
{
    public Vector3 offset;
    public float waitTime;
}

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private STATS stats;

    public List<PatrolPoint> patrolPoints = new();
    private int _currentPatrolPoint = 0;
    private Vector3 _startPosition;

    private Tween _currentMovementTween;
    private WaitForSeconds _wait = new(1f);

    private void Start()
    {
        _startPosition = transform.position;
        if (patrolPoints.Count > 0)
        {
            transform.position = _startPosition + patrolPoints[0].offset;
            IteratePatrolPoint();
            Patrol();
        }
    }

    private void Patrol()
    {
        var distanceToNextPoint =
            Vector3.Distance(transform.position, _startPosition + patrolPoints[_currentPatrolPoint].offset);

        _currentMovementTween = transform.DOMove(_startPosition + patrolPoints[_currentPatrolPoint].offset,
            distanceToNextPoint / stats.ST_Speed * .5f).SetEase(Ease.InOutQuad);
        _currentMovementTween.onComplete +=
            () => { StartCoroutine(WaitPatrol()); };
    }

    public void InterruptPatrol()
    {
        StartCoroutine(InterruptPatrolRoutine());
    }

    private IEnumerator InterruptPatrolRoutine()
    {
        _currentMovementTween.Kill();
        yield return _wait;
        Patrol();
    }

    private void IteratePatrolPoint()
    {
        if (_currentPatrolPoint < patrolPoints.Count - 1)
            _currentPatrolPoint++;
        else
            _currentPatrolPoint = 0;
    }

    private void OnDisable()
    {
        DOTween.Kill(transform);
    }

    private IEnumerator WaitPatrol()
    {
        yield return new WaitForSecondsRealtime(patrolPoints[_currentPatrolPoint].waitTime);
        IteratePatrolPoint();
        Patrol();
    }

    private void OnDrawGizmos()
    {
        /*
        if (patrolPoints.Count > 0)
            _startPosition = transform.position;
        */

        for (var i = 0; i < patrolPoints.Count; i++)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position + patrolPoints[i].offset, 0.5f);
        }
    }
}