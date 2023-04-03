using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheckpointHandler : MonoBehaviour
{
    private bool _checkForCheckpoint = true;
    private float _timeStandingStill;
    [SerializeField] private float maxDistanceToCheckpoint = 10f;
    public Transform _staticCheckpoint;
    private CheckpointScript[] _checkpoints;
    private int _currentCheckpoint;
    private PlayerMovement _playerMovement;
    private PlayerReceiveInput _playerReceiveInput;
    private WaitForSeconds _delay = new(0.1f);

    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerReceiveInput = GetComponent<PlayerReceiveInput>();
        _checkpoints = LevelManager.GetCheckpoints().ToArray();

        StartCoroutine(Coroutine_CheckCheckpoints());
    }


    private void CheckIfCheckpointsApply()
    {
        _checkForCheckpoint = false;
        _timeStandingStill = 0;
        var finalCheckpointPos = Vector3.zero;
        var maxDist = maxDistanceToCheckpoint;

        if (_staticCheckpoint != null)
        {
            finalCheckpointPos = _staticCheckpoint.transform.position;
            maxDist = 6f;
        }
        else
        {
            if (_checkpoints.Length == 0) return;

            if (_currentCheckpoint == _checkpoints.Length) return;

            if (_checkpoints[_currentCheckpoint].isActivated ||
                _checkpoints[_currentCheckpoint].pauseCheckpoint ||
                _currentCheckpoint > _checkpoints.Length) return;

            finalCheckpointPos = _checkpoints[_currentCheckpoint].transform.position;
            //maxDist = maxDistanceToCheckpoint;
        }

        if (Vector3.Distance(transform.position, finalCheckpointPos) <
            maxDist && _playerMovement.inputDirectionTo.magnitude < 0.1f)
        {
            if (finalCheckpointPos.x > transform.position.x &&
                _playerMovement.isFacingRight == -1)
                _playerMovement.FlipPlayer(1, 0.5f);
            else if (finalCheckpointPos.x < transform.position.x &&
                     _playerMovement.isFacingRight == 1)
                _playerMovement.FlipPlayer(-1, 0.5f);
        }
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

    private IEnumerator Coroutine_CheckCheckpoints()
    {
        while (true)
        {
            if (_playerMovement.inputDirectionTo.magnitude < 0.1f)
            {
                _timeStandingStill += Time.deltaTime;
                if (_timeStandingStill > 0.1f)
                    _checkForCheckpoint = true;
            }
            else
            {
                _timeStandingStill = 0;
                _checkForCheckpoint = false;
            }

            if (_checkForCheckpoint)
            {
                _checkpoints = LevelManager.GetCheckpoints().ToArray();
                CheckIfCheckpointsApply();
            }

            yield return _delay;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            var checkpoint = other.GetComponent<CheckpointScript>();

            if (checkpoint.isThisCheckPointStatic)
                _staticCheckpoint = checkpoint.transform;
            else if (checkpoint.isActivated == false)
                if (IncreaseCheckpoint(checkpoint.checkpointNumber))
                    checkpoint.isActivated = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
            _staticCheckpoint = null;
    }
}