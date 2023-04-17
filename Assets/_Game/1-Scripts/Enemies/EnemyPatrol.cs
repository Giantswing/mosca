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

public class EnemyPatrol : MonoBehaviour, ICustomTeleport
{
    public List<PatrolPoint> patrolPoints = new();
    private int _currentPatrolPoint = 0;
    private Vector3 _startPosition;
    private Vector3 _startLastMovement;
    private Attributes attributes;

    private Tween _currentMovementTween;
    private WaitForSeconds _wait = new(1f);
    private Transform LastTeleporterUsed;

    private List<Color> patrolColors = new()
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        Color.cyan,
        Color.magenta
    };

    private void Awake()
    {
        attributes = GetComponent<Attributes>();
    }

    private void Start()
    {
        _startPosition = transform.position;
        if (patrolPoints.Count > 0)
        {
            transform.position = _startPosition + patrolPoints[0].offset;
            IteratePatrolPoint();
            Patrol();
        }

        patrolColors.Clear();
        for (var i = 0; i < patrolPoints.Count; i++)
            patrolColors.Add(new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f)));
    }

    private void Patrol(bool isImmediate = false)
    {
        _startLastMovement = transform.position;
        Debug.DrawLine(transform.position, transform.position + Vector3.up,
            Color.magenta, 1f);
        //print(_startLastMovement);
        Ease movementEase = isImmediate ? Ease.OutQuad : Ease.InOutQuad;
        float distanceToNextPoint =
            Vector3.Distance(transform.position,
                _startPosition + patrolPoints[_currentPatrolPoint].offset);


        _currentMovementTween = transform.DOMove(
            _startPosition + patrolPoints[_currentPatrolPoint].offset,
            distanceToNextPoint / attributes.speed * .5f).SetEase(movementEase);
        _currentMovementTween.onComplete +=
            () => { StartCoroutine(WaitPatrol()); };


        /*
        _currentMovementTween = transform.DOMove(
            CalculateEndingPos(_currentPatrolPoint),
            distanceToNextPoint / stats.ST_Speed * .5f).SetEase(movementEase);
        _currentMovementTween.onComplete +=
            () => { StartCoroutine(WaitPatrol()); };
            */
    }

    private Vector3 CalculateEndingPos(int PatrolPoint)
    {
        Vector3 result = _startPosition;
        for (var i = 0; i < PatrolPoint; i++) result += patrolPoints[i].offset;

        return result;
    }

    public void InterruptPatrol()
    {
        StartCoroutine(InterruptPatrolRoutine());
    }


    public void CustomTeleport(Transform teleporterTransform, Transform originalTeleporterTransform)
    {
        Vector3 original_difference = originalTeleporterTransform.position - transform.position;
        original_difference = Vector3.zero;


        foreach (PatrolPoint patrolPoint in patrolPoints)
        {
            Vector3 originalOffset = originalTeleporterTransform.position - _startPosition;
            Vector3 distToPortal = _startPosition + patrolPoint.offset - originalTeleporterTransform.position +
                                   original_difference;

            Debug.DrawLine(originalTeleporterTransform.position, originalTeleporterTransform.position + distToPortal,
                patrolColors[patrolPoints.IndexOf(patrolPoint)], 1.5f);


            Vector3 newOffset = Quaternion.Euler(0, 0,
                                    teleporterTransform.rotation.eulerAngles.z -
                                    originalTeleporterTransform.rotation.eulerAngles.z
                                ) *
                                -distToPortal;

            newOffset += originalOffset;
            patrolPoint.offset = newOffset - original_difference;

            patrolPoint.offset += teleporterTransform.position - originalTeleporterTransform.position;
        }


        transform.position = teleporterTransform.position - original_difference;
        _currentMovementTween.Kill();
        Patrol(true);
    }

    public GameObject ReturnGameobject()
    {
        return gameObject;
    }

    private void TestRay(Vector3 pos)
    {
        Debug.DrawLine(pos, pos + Vector3.up * 10, Color.red, 1.5f);
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
        Vector3 pos = Application.isPlaying ? _startPosition : transform.position;

        for (var i = 0; i < patrolPoints.Count; i++)
        {
            Gizmos.color = patrolColors[i];
            Gizmos.DrawWireSphere(pos + patrolPoints[i].offset, 0.5f);
        }
    }
}