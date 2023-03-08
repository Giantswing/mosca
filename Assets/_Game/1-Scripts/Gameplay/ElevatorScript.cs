using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using HoudiniEngineUnity;
using UnityEngine;

[Serializable]
public class ElevatorMovePoint
{
    public Vector3 offset;
    public float waitTime;
    public bool openLeftDoor;
    public bool openRightDoor;
    public float moveSpeed;
}

[ExecuteInEditMode]
public class ElevatorScript : MonoBehaviour
{
    [SerializeField] public float xSize, ySize;
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private ElevatorScriptDoor[] doors;
    [SerializeField] private EventCaller eventCaller;
    [SerializeField] private float doorHeight = 2f;
    [SerializeField] private PhysicMaterial physicMaterial;

    public List<ElevatorMovePoint> MovePoints = new();
    public HEU_HoudiniAssetRoot assetRoot;

    private bool _changedThisFrame;
    private int _currentMovePoint;
    private int _nextMovePoint;
    private int _direction = 1;
    private Vector3 _startPosition;
    private WaitForSeconds[] _waitTimes;

    private ParentChanger _parentChanger;

    /*
    [SerializeField] private List<GameObject> entranceBlocks = new();
    [SerializeField] private GameObject entranceBlockPrefab;
    */

    private void Start()
    {
        _startPosition = transform.position;
        _waitTimes = new WaitForSeconds[MovePoints.Count];
        UpdateDoors();

        var children = GetComponentsInChildren<MeshCollider>();
        foreach (var child in children) child.material = physicMaterial;
    }

    /*
    private void CreateEntranceBlocks()
    {
        DeleteEntranceBlocks();
        for (var i = 0; i < MovePoints.Count; i++)
            if (MovePoints[i].openLeftDoor || MovePoints[i].openRightDoor)
            {
                print("spawned entrance");
                var horizontalOffset = MovePoints[i].openRightDoor ? 1f : -1f;
                var entranceBlock = Instantiate(entranceBlockPrefab);
                entranceBlock.transform.position =
                    transform.position + transform.right * (4.1f * horizontalOffset + xSize * horizontalOffset * 10f) +
                    transform.up * -2.9f;
                entranceBlocks.Add(Instantiate(entranceBlockPrefab));
            }
    }

    private void DeleteEntranceBlocks()
    {
        foreach (var entranceBlock in entranceBlocks)
            Destroy(entranceBlock);

        entranceBlocks.Clear();
    }
    */

    private void OnEnable()
    {
        eventCaller.OnStartEvent += StartMoving;

        if (!Application.isPlaying)
        {
            if (assetRoot != null)
                assetRoot.HoudiniAsset.RequestCook();


            var expectedDoors = GetComponentsInChildren<ElevatorScriptDoor>();
            if (expectedDoors.Length == 0)
            {
                if (doorPrefab != null)
                {
                    doors = new ElevatorScriptDoor[2];
                    doors[0] = Instantiate(doorPrefab, transform).GetComponent<ElevatorScriptDoor>();
                    doors[1] = Instantiate(doorPrefab, transform).GetComponent<ElevatorScriptDoor>();
                    doors[0].transform.position = transform.position;
                    doors[1].transform.position = transform.position;
                }
            }
            else
            {
                doors = new ElevatorScriptDoor[2];
                doors[0] = expectedDoors[0];
                doors[1] = expectedDoors[1];
            }

            doors[0].isDoorOpen = true;
            doors[1].isDoorOpen = true;
        }

        //CreateEntranceBlocks();
    }

    private void OnDisable()
    {
        eventCaller.OnStartEvent -= StartMoving;
        //DeleteEntranceBlocks();
    }

    private void CloseDoors()
    {
        doors[0].CloseDoor(Move);
        doors[1].CloseDoor(Move);
    }

    private void OpenDoors()
    {
        foreach (var door in doors)
            door.OpenDoor();
    }

    private void UpdateDoors()
    {
        if (MovePoints[_currentMovePoint].openLeftDoor)
            doors[0].OpenDoor();
        else
            doors[0].CloseDoor();

        if (MovePoints[_currentMovePoint].openRightDoor)
            doors[1].OpenDoor();
        else
            doors[1].CloseDoor();
    }

    private void StartMoving()
    {
        CloseDoors();
    }

    private void Move()
    {
        _parentChanger = GetComponentInChildren<ParentChanger>();
        if (_parentChanger.IsSomeoneInside())
        {
            transform.DOShakeRotation(3f, 2f, 30, 90f, true).SetUpdate(UpdateType.Fixed);
            IterateMovePoint();
            var distanceToNextPoint =
                Vector3.Distance(transform.position, _startPosition + MovePoints[_currentMovePoint].offset);

            var movementDuration = distanceToNextPoint / MovePoints[_currentMovePoint].moveSpeed;


            transform.DOMove(_startPosition + MovePoints[_currentMovePoint].offset,
                    movementDuration).SetEase(Ease.InOutCubic).SetUpdate(UpdateType.Fixed).onComplete +=
                () => { StartCoroutine(WaitMove()); };
        }
        else
        {
            if (MovePoints[_currentMovePoint].openRightDoor)
                doors[1].OpenDoor();

            if (MovePoints[_currentMovePoint].openLeftDoor)
                doors[0].OpenDoor();
        }
    }

    private IEnumerator WaitMove()
    {
        transform.DOShakeRotation(0.5f, 2f, 30, 90f, true).SetUpdate(UpdateType.Fixed);
        yield return _waitTimes[_currentMovePoint];
        UpdateDoors();

/*
        if (IterateMovePoint()) Move();
        UpdateDoors();
*/

        print(_currentMovePoint);
    }

    private bool IterateMovePoint()
    {
        if (_direction == 1)
        {
            if (_currentMovePoint < MovePoints.Count - 2)
            {
                _currentMovePoint++;
                return true;
            }
            else
            {
                _currentMovePoint = MovePoints.Count - 1;
                _direction = -1;
                return false;
            }
        }

        if (_direction == -1)
        {
            if (_currentMovePoint > 0)
            {
                _currentMovePoint--;
                return true;
            }
            else
            {
                _currentMovePoint = 1;
                _direction = 1;
                return false;
            }
        }

        return false;
    }

    public void UpdateParameters()
    {
        //print("this is a callback");

        var parameters = assetRoot.HoudiniAsset.Parameters.GetParameters();

        foreach (var parameter in parameters)
        {
            if (parameter._name == "sizeWidth")
                xSize = parameter._floatValues[0];
            if (parameter._name == "sizeHeight")
                ySize = parameter._floatValues[0];
        }

        /*
        doors[0].transform.position = transform.position + new Vector3(-4.1f - xSize * 10f, -2.9f + doorHeight, 0);
        doors[1].transform.position = transform.position + new Vector3(4.1f + xSize * 10f, -2.9f + doorHeight, 0);
        */


        //transform depending on elevator transform right
        doors[0].transform.position =
            transform.position + transform.right * (-4.1f - xSize * 10f) +
            transform.up * (-2.9f + doorHeight);


        doors[1].transform.position =
            transform.position + transform.right * (4.1f + xSize * 10f) +
            transform.up * (-2.9f + doorHeight);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            _startPosition = transform.position + new Vector3(0, 0.5f + ySize * .5f, 0);

        Gizmos.color = Color.magenta;
        for (var i = 0; i < MovePoints.Count; i++)
        {
            var point = _startPosition + MovePoints[i].offset;
            Gizmos.DrawWireCube(point, new Vector3(8.8f + xSize * 20f, 8.7f + ySize * 1f, 4f));
            if (i > 0)
            {
                var prevPoint = _startPosition + MovePoints[i - 1].offset;
                Gizmos.DrawLine(prevPoint, point);
            }
        }
    }
}