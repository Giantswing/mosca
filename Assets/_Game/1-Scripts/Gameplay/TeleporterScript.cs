using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeleporterScript : MonoBehaviour
{
    [SerializeField] private bool isEnabled = true;
    [SerializeField] private TeleporterScript otherTeleporter;
    [SerializeField] private bool isOnlyExit = false;
    [SerializeField] private float teleportDelay = 0.35f;
    [SerializeField] private bool main = true;
    [SerializeField] private ParticleSystem teleportParticles;
    private PlayerMovement _playerMovement;
    private WaitForSeconds _teleportDelayWait;
    private float _forceMultiplier = 2.5f;
    private Vector2 previousPlayerDirection;


    [Space(10)] [SerializeField] private SimpleAudioEvent teleportSoundEvent;
    [SerializeField] private AudioSource teleportSoundSource;

    private enum TeleportDirection
    {
        Up,
        Down,
        Left,
        Right
    };

    [SerializeField] private TeleportDirection teleportDirection;

    private void Start()
    {
        _teleportDelayWait = new WaitForSeconds(teleportDelay);
    }

    public void Teleport(GameObject target)
    {
        if (!isEnabled || otherTeleporter == null) return;

        _playerMovement = target.GetComponent<PlayerMovement>();
        var outputDir = otherTeleporter.CalculateDirection();

        target.transform.position =
            otherTeleporter.transform.position + new Vector3(outputDir.x * 0.75f, outputDir.y * 0.75f, 0);

        _playerMovement.hSpeed = outputDir.x * _forceMultiplier;
        _playerMovement.vSpeed = outputDir.y * _forceMultiplier;
        //_playerMovement.inputDirectionTo = Vector2.zero;
        previousPlayerDirection = _playerMovement.inputDirectionTo;
        _playerMovement.inputDirectionTo = outputDir;

        otherTeleporter.StartTeleportCooldownCoroutine(_playerMovement);
    }

    public void PlayParticles()
    {
        teleportParticles.Emit(25);
    }

    public Vector2 CalculateDirection()
    {
        Vector2 outputDir;
        switch (teleportDirection)
        {
            case TeleportDirection.Up:
                outputDir = Vector2.up;
                break;
            case TeleportDirection.Down:
                outputDir = Vector2.down;
                break;
            case TeleportDirection.Left:
                outputDir = Vector2.left;
                break;
            case TeleportDirection.Right:
                outputDir = Vector2.right;
                break;
            default:
                outputDir = Vector2.zero;
                break;
        }

        return outputDir;
    }

    public void StartTeleportCooldownCoroutine(PlayerMovement playerMov)
    {
        StartCoroutine(TeleportCooldownCoroutine(playerMov));
    }

    private IEnumerator TeleportCooldownCoroutine(PlayerMovement playerMov)
    {
        isEnabled = false;
        teleportSoundEvent.Play(teleportSoundSource);
        PlayParticles();
        playerMov.GetComponent<PlayerInput>().enabled = false;
        yield return _teleportDelayWait;
        playerMov.GetComponent<PlayerInput>().enabled = true;

        if (playerMov.inputDirectionTo != Vector2.zero)
            playerMov.inputDirectionTo = previousPlayerDirection;

        isEnabled = true;
    }

    private void OnDrawGizmos()
    {
        if (otherTeleporter == null) return;

        if (main)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, otherTeleporter.transform.position);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + Vector3.down * 0.2f,
                otherTeleporter.transform.position + Vector3.down * 0.2f);
        }
    }
}