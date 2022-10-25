using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    private BoxCollider _teleportCollider;
    private Vector3 _originalSize;
    private MeshRenderer _meshRenderer;
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
        _teleportCollider = GetComponent<BoxCollider>();
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _originalSize = transform.localScale;

        if (isOnlyExit)
        {
            _meshRenderer.enabled = false;
            _teleportCollider.enabled = false;
        }
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
        if (isOnlyExit)
        {
            transform.localScale = _originalSize;
            _meshRenderer.enabled = true;
            transform.DOScale(0, .65f).SetEase(Ease.InElastic).SetDelay(0.1f).onComplete +=
                () => _meshRenderer.enabled = false;
        }

        isEnabled = false;
        teleportSoundEvent.Play(teleportSoundSource);
        PlayParticles();
        FreezeFrameScript.DistortView(0.2f);
        playerMov.GetComponent<PlayerInput>().enabled = false;
        yield return _teleportDelayWait;
        playerMov.GetComponent<PlayerInput>().enabled = true;

        if (playerMov.inputDirectionTo != Vector2.zero)
            playerMov.inputDirectionTo = previousPlayerDirection;

        isEnabled = !isOnlyExit;
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