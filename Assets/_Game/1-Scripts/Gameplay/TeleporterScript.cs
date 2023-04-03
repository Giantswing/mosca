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
    [SerializeField] private BoxCollider myCollider;
    [SerializeField] private float TimeToActivateAgain = 1f;
    private WaitForSeconds _WaitTimeToActivateAgain;


    [Space(10)] [SerializeField] private SimpleAudioEvent teleportSoundEvent;
    [SerializeField] private AudioSource teleportSoundSource;

    /*
    private enum TeleportDirection
    {
        Up,
        Down,
        Left,
        Right
    };

    [SerializeField] private TeleportDirection teleportDirection;
    */

    [Space(15)] [SerializeField] private float teleportStrength = 3f;
    [SerializeField] private float teleportDuration = .5f;

    private void Awake()
    {
        myCollider = GetComponentInChildren<BoxCollider>();
        _WaitTimeToActivateAgain = new WaitForSeconds(TimeToActivateAgain);
    }

    private void Start()
    {
        _teleportDelayWait = new WaitForSeconds(teleportDelay);

        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _originalSize = transform.localScale;

        if (isOnlyExit)
        {
            _meshRenderer.enabled = false;
            myCollider.enabled = false;
            _meshRenderer.gameObject.SetActive(false);
        }
    }

    public void Teleport(GameObject target)
    {
        if (!isEnabled || otherTeleporter == null) return;

        _playerMovement = target.GetComponent<PlayerMovement>();
        var outputDir = transform.right;

        target.transform.position =
            otherTeleporter.transform.position + new Vector3(outputDir.x * 0.75f, outputDir.y * 0.75f, 0);

        _playerMovement.hSpeed = outputDir.x * _forceMultiplier;
        _playerMovement.vSpeed = outputDir.y * _forceMultiplier;
        //_playerMovement.inputDirectionTo = Vector2.zero;
        previousPlayerDirection = _playerMovement.inputDirectionTo;
        _playerMovement.inputDirectionTo = outputDir;

        otherTeleporter.StartTeleportCooldownCoroutine(_playerMovement);
    }

    private IEnumerator NewTeleport(GameObject target)
    {
        myCollider.enabled = false;
        otherTeleporter.myCollider.enabled = false;

        if (!isEnabled || otherTeleporter == null) yield return null;

        var zDifference = target.transform.position.z - transform.position.z;

        //var outputDir = otherTeleporter.CalculateDirection();
        var outputDir = otherTeleporter.transform.right;

        PlayParticles();
        transform.DOShakeRotation(0.5f, 10f, 10, 90f, false);
        transform.DOShakeScale(0.5f, 0.1f, 10, 90f, false);
        target.transform.DOShakeScale(0.5f, 0.5f, 10, 90f, false);


        if (target.TryGetComponent(out ICustomTeleport customTeleport))
            /*
            target.transform.position = otherTeleporter.transform.position +
                                        new Vector3(outputDir.x * 1.35f, outputDir.y * 1.35f, zDifference);
                                        */
            customTeleport.CustomTeleport(otherTeleporter.transform, transform);
        else if (target.TryGetComponent(out Rigidbody rb))
            RigidbodyTeleport(rb, target, outputDir, zDifference);
        else
            NormalTeleport(target, outputDir, zDifference);


        if (otherTeleporter.isOnlyExit)
        {
            otherTeleporter.myCollider.enabled = false;
            otherTeleporter.transform.localScale = _originalSize;
            otherTeleporter._meshRenderer.enabled = true;
            otherTeleporter._meshRenderer.gameObject.SetActive(true);
            otherTeleporter.transform.DOScale(0, 1.2f).SetEase(Ease.InElastic).SetDelay(0.1f).onComplete +=
                () =>
                {
                    otherTeleporter._meshRenderer.enabled = false;
                    otherTeleporter._meshRenderer.gameObject.SetActive(false);
                };
        }
        else
        {
            DOVirtual.DelayedCall(0.25f, () =>
            {
                otherTeleporter.transform.DOShakeRotation(0.5f, 10f, 10, 90f, false);
                otherTeleporter.transform.DOShakeScale(0.5f, 0.1f, 10, 90f, false);
            });
        }

        teleportSoundEvent.Play(teleportSoundSource);
        otherTeleporter.PlayParticles();

        yield return _WaitTimeToActivateAgain;

        if (!isOnlyExit)
            myCollider.enabled = true;
        otherTeleporter.myCollider.enabled = true;
    }

    private void RigidbodyTeleport(Rigidbody rb, GameObject target, Vector3 outputDir, float zDifference)
    {
        var rbVelocity = rb.velocity;
        var localVelocity = transform.InverseTransformDirection(rbVelocity);
        var rotatedVelocity =
            Quaternion.FromToRotation(transform.forward, otherTeleporter.transform.forward) * -localVelocity;
        var worldVelocity = otherTeleporter.transform.TransformDirection(rotatedVelocity);
        target.transform.position = otherTeleporter.transform.position +
                                    new Vector3(outputDir.x * 0.75f, outputDir.y * 0.75f, zDifference);
        rb.velocity = worldVelocity;
    }

    private void NormalTeleport(GameObject target, Vector3 outputDir, float zDifference)
    {
        target.transform.position = otherTeleporter.transform.position +
                                    new Vector3(outputDir.x * -0.35f, outputDir.y * -0.35f, zDifference);


        target.transform.DOLocalMove(
            otherTeleporter.transform.position + new Vector3(outputDir.x * teleportStrength,
                outputDir.y * teleportStrength, zDifference),
            teleportDuration).SetEase(Ease.InBack);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out STATS stats))
        {
            if (stats.canBeTeleported && !IgnoredParentObjects.shouldBeIgnored(stats.transform))
                StartCoroutine(NewTeleport(stats.gameObject));
        }
        else if (other.gameObject.TryGetComponent(out ICustomTeleport customTeleport))
        {
            StartCoroutine(NewTeleport(customTeleport.ReturnGameobject()));
        }
    }


    public void PlayParticles()
    {
        DOVirtual.DelayedCall(0.2f, () => { teleportParticles.Emit(25); });
    }

    /*
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
    */

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
            _meshRenderer.gameObject.SetActive(true);
            transform.DOScale(0, 1.2f).SetEase(Ease.InElastic).SetDelay(0.1f).onComplete +=
                () =>
                {
                    _meshRenderer.enabled = false;
                    _meshRenderer.gameObject.SetActive(false);
                };
        }

        isEnabled = false;
        teleportSoundEvent.Play(teleportSoundSource);
        PlayParticles();
        ScreenFXSystem.DistortView(0.2f);
        playerMov.GetComponent<PlayerInput>().enabled = false;
        yield return _teleportDelayWait;
        playerMov.GetComponent<PlayerInput>().enabled = true;

        if (playerMov.inputDirectionTo != Vector2.zero)
            playerMov.inputDirectionTo = previousPlayerDirection;

        isEnabled = !isOnlyExit;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * 2f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 2f);


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