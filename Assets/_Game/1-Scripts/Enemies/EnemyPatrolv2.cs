using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class PatrolPointRot
{
    public Vector3 rot;
    public float duration;
    public float waitTime;
}


public class EnemyPatrolv2 : MonoBehaviour, ICustomTeleport
{
    [SerializeField] private STATS stats;

    public List<PatrolPointRot> patrolPoints = new();
    private List<PatrolPoint> originalPatrolPoints = new();
    private int _currentPatrolPoint = 0;
    private Vector3 _startPosition;
    private Vector3 currentDir;

    private Tween _currentMovementTween;
    private WaitForSeconds _wait = new(1f);

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
        stats = GetComponent<STATS>();
    }

    public GameObject ReturnGameobject()
    {
        return gameObject;
    }

    private void Start()
    {
        _startPosition = transform.position;

        if (patrolPoints.Count > 0)
        {
            transform.position = _startPosition + CalculateLengthOfPatrolPoint(0);
            IteratePatrolPoint();
            Patrol();
        }

        patrolColors.Clear();
        for (var i = 0; i < patrolPoints.Count; i++)
            patrolColors.Add(new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f)));
    }

    private Vector3 CalculateLengthOfPatrolPoint(int index)
    {
        return transform.right * stats.ST_Speed * patrolPoints[index].duration;
    }

    private void Patrol(bool isImmediate = false)
    {
        var movementEase = isImmediate ? Ease.OutQuad : Ease.InOutQuad;
        /*
        var distanceToNextPoint =
            Vector3.Distance(transform.position, _startPosition + patrolPoints[_currentPatrolPoint].offset);

        _currentMovementTween = transform.DOMove(_startPosition + patrolPoints[_currentPatrolPoint].offset,
            distanceToNextPoint / stats.ST_Speed * .5f).SetEase(movementEase);
        _currentMovementTween.onComplete +=
            () => { StartCoroutine(WaitPatrol()); };
        */


        _currentMovementTween = transform.DOMove(_startPosition + CalculateLengthOfPatrolPoint(_currentPatrolPoint),
            patrolPoints[_currentPatrolPoint].duration).SetEase(movementEase);

        _currentMovementTween.onComplete +=
            () => { StartCoroutine(WaitPatrol()); };
    }

    public void InterruptPatrol()
    {
        StartCoroutine(InterruptPatrolRoutine());
    }

    /*
    public void CustomTeleport(Transform teleporterTransform, Transform originalTeleporterTransform)
    {
        var original_difference = originalTeleporterTransform.position - transform.position;
        var originalTeleporterPosition = transform.position;
        transform.position = teleporterTransform.position - original_difference;

        _currentMovementTween.Kill();

        foreach (var patrolPoint in patrolPoints)
        {
            original_difference = Vector3.zero;
            var ported = teleporterTransform.position + patrolPoint.offset -
                         originalTeleporterPosition - original_difference;

            TestRay(ported);

            var distToPortal = patrolPoint.offset + _startPosition - originalTeleporterPosition;

            patrolPoint.offset = teleporterTransform.right * distToPortal.x +
                                 teleporterTransform.up * distToPortal.y;

            patrolPoint.offset += teleporterTransform.position - originalTeleporterPosition;
        }


        Patrol(true);
    }
    */
    public void CustomTeleport(Transform teleporterTransform, Transform originalTeleporterTransform)
    {
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
        var pos = Application.isPlaying ? _startPosition : transform.position;

        for (var i = 0; i < patrolPoints.Count; i++)
        {
            Gizmos.color = patrolColors[i];
            Gizmos.DrawWireSphere(pos + CalculateLengthOfPatrolPoint(i), .5f);


            Gizmos.color = Color.white;
            Gizmos.DrawLine(pos + CalculateLengthOfPatrolPoint(i),
                pos + CalculateLengthOfPatrolPoint(i) + patrolPoints[i].rot * 2);
        }
    }
}