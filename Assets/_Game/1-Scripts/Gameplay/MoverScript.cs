using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class MovePoint
{
    public Vector3 offset;
    public Vector3 rotation;
    public float waitTime;
}

public class MoverScript : MonoBehaviour, IPressurePlateListener
{
    [SerializeField] private bool pingPong;
    [SerializeField] private bool smoothMove = true;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip moveSound;

    [SerializeField] private Transform my3dModel;
    [SerializeField] private Transform myParent;
    [SerializeField] private GameObject moverRailPrefab;
    [SerializeField] private GameObject[] moverRails;

    [SerializeField] private bool isAutomatic = true;


    public List<MovePoint> MovePoints = new();


    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float duration;
    private bool _changedThisFrame;
    private int _currentMovePoint;
    private int _direction = 1;
    private Vector3 _startPosition;
    private WaitForSeconds[] _waitTimes;
    private Ease ease;


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
            if (isAutomatic == false) return;
            transform.position = _startPosition + MovePoints[0].offset;
            IterateMovePoint();
            Move();
        }
    }

    private int CheckRotationDirection(Vector3 pos1, Vector3 pos2)
    {
        if (pos1.x > pos2.x || pos1.y < pos2.y)
            return 1;
        else
            return -1;
    }

    private void OnDisable()
    {
        transform.DOKill();
    }

    public void AutoMove()
    {
        IterateMovePoint();
        Move();
    }

    /*
    private void OnDestroy()
    {
        if (gameObject.name == "Mover")
            gameObject.name = "Mover (0)";

        //find objects with tag moverail
        var rails = GameObject.FindGameObjectsWithTag("MoveRail");

        foreach (var rail in rails)
            if (rail.name.StartsWith(gameObject.name))
                EditorApplication.delayCall += () => { DestroyImmediate(rail); };
    }
    */

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
            var point2 = point + Quaternion.Euler(MovePoints[i].rotation) * transform.up * 0.5f;

            Gizmos.DrawSphere(point, .2f);

            Gizmos.DrawLine(point, point2);
        }
    }

    private void OnValidate()
    {
        //SpawnRails();
    }

    /*
    private void SpawnRails()
    {
        _startPosition = transform.position;
        /*
        //clear old rails if there are any
        if (moverRails.Length > 0)
            foreach (var rail in moverRails)
                if (rail != null)
                    EditorApplication.delayCall += () => { DestroyImmediate(rail); };


        if (gameObject.name == "Mover")
            gameObject.name = "Mover (0)";

        //find objects with tag moverail
        var rails = GameObject.FindGameObjectsWithTag("MoveRail");

        foreach (var rail in rails)
            if (rail.name.StartsWith(gameObject.name))
                EditorApplication.delayCall += () => { DestroyImmediate(rail); };


        //spawn new rails
        moverRails = new GameObject[MovePoints.Count - 1];
        for (var i = 0; i < MovePoints.Count - 1; i++)
        {
            moverRails[i] = Instantiate(moverRailPrefab);
            moverRails[i].name = gameObject.name + " Rail " + i;
            moverRails[i].transform.position = _startPosition + MovePoints[i].offset;

            //create rotation based on the two points position
            moverRails[i].transform.rotation = Quaternion.LookRotation(
                _startPosition + MovePoints[i + 1].offset - (_startPosition + MovePoints[i].offset), Vector3.up);
            moverRails[i].transform.Rotate(90f, 0, 0);

            var scale = Vector3.Distance(_startPosition + MovePoints[i].offset,
                _startPosition + MovePoints[i + 1].offset);
            moverRails[i].transform.localScale = new Vector3(1, scale * 0.5f, 1);
        }
    }
*/
    private void IterateMovePoint()
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(moveSound);

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
        //_startPosition = Vector3.zero;

        var distanceToNextPoint =
            Vector3.Distance(transform.position, _startPosition + MovePoints[_currentMovePoint].offset);


        /*
        var distanceToNextPoint =
            Vector3.Distance(myParent.position, myParent.position + MovePoints[_currentMovePoint].offset);
            */

        var movementDuration = distanceToNextPoint / moveSpeed;

        myParent.DOLocalRotate(MovePoints[_currentMovePoint].rotation, movementDuration).SetEase(Ease.Linear);

        if (duration == 0)
        {
            transform.DOMove(_startPosition + MovePoints[_currentMovePoint].offset,
                    movementDuration).SetEase(ease).onComplete +=
                () => { StartCoroutine(WaitMove()); };


            my3dModel.DORotate(
                new Vector3(0, 0,
                    my3dModel.rotation.eulerAngles.z + 10 *
                    CheckRotationDirection(transform.position, _startPosition + MovePoints[_currentMovePoint].offset) *
                    distanceToNextPoint * 15),
                movementDuration,
                RotateMode.FastBeyond360).SetEase(ease);
        }
        else
        {
            transform.DOMove(_startPosition + MovePoints[_currentMovePoint].offset,
                    duration).SetEase(ease).onComplete +=
                () => { StartCoroutine(WaitMove()); };
        }
    }

    private IEnumerator WaitMove()
    {
        if (isAutomatic)
        {
            yield return _waitTimes[_currentMovePoint];

            IterateMovePoint();
            Move();
        }

        yield return null;
    }
}