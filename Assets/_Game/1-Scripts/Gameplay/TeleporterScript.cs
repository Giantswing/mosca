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
    private Vector3 startLocalRotation;


    [Space(10)] [SerializeField] private SimpleAudioEvent teleportSoundEvent;
    [SerializeField] private AudioSource teleportSoundSource;

    [Space(15)] [SerializeField] private float teleportStrength = 3f;
    [SerializeField] private float teleportDuration = .5f;

    private void Awake()
    {
        myCollider = GetComponentInChildren<BoxCollider>();
        _WaitTimeToActivateAgain = new WaitForSeconds(TimeToActivateAgain);
        startLocalRotation = transform.localEulerAngles;
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

    /*
    public void Teleport(GameObject target)
    {
        if (!isEnabled || otherTeleporter == null) return;

        _playerMovement = target.GetComponent<PlayerMovement>();
        Vector3 outputDir = transform.right;

        target.transform.position =
            otherTeleporter.transform.position + new Vector3(outputDir.x * 0.75f, outputDir.y * 0.75f, 0);

        _playerMovement.hSpeed = outputDir.x * _forceMultiplier;
        _playerMovement.vSpeed = outputDir.y * _forceMultiplier;
        //_playerMovement.inputDirectionTo = Vector2.zero;
        previousPlayerDirection = _playerMovement.inputDirectionTo;
        _playerMovement.inputDirectionTo = outputDir;

        otherTeleporter.StartTeleportCooldownCoroutine(_playerMovement);
    }
    */

    private IEnumerator NewTeleport(GameObject target)
    {
        otherTeleporter.myCollider.enabled = false;

        if (!isEnabled || otherTeleporter == null) yield return null;

        float zDifference = target.transform.position.z - transform.position.z;
        Vector3 outputDir = otherTeleporter.transform.right;

        PlayParticles();
        transform.DOShakeRotation(0.5f, 10f, 10, 90f, false).onComplete += () =>
        {
            transform.localEulerAngles = startLocalRotation;
        };
        transform.DOShakeScale(0.5f, 0.1f, 10, 90f, false);
        target.transform.DOShakeScale(0.5f, 0.5f, 10, 90f, false);


        if (target.TryGetComponent(out ICustomTeleport customTeleport))
            customTeleport.CustomTeleport(otherTeleporter.transform, transform);
        else if (target.TryGetComponent(out Rigidbody rb))
            RigidbodyTeleport(rb, target, outputDir, zDifference);


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

        teleportSoundEvent.Play(otherTeleporter.teleportSoundSource);
        otherTeleporter.PlayParticles();

        yield return _WaitTimeToActivateAgain;

        otherTeleporter.myCollider.enabled = true;
    }

    private void RigidbodyTeleport(Rigidbody rb, GameObject target, Vector3 outputDir, float zDifference)
    {
        print("rigidbody teleport");
        Vector3 rbVelocity = rb.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(rbVelocity);
        Vector3 rotatedVelocity =
            Quaternion.FromToRotation(transform.forward, otherTeleporter.transform.forward) * -localVelocity;
        Vector3 worldVelocity = otherTeleporter.transform.TransformDirection(rotatedVelocity);
        target.transform.position = otherTeleporter.transform.position +
                                    new Vector3(outputDir.x * 0.15f, outputDir.y * 0.15f, zDifference);

        rb.velocity = worldVelocity;
        rb.AddForce(outputDir * teleportStrength, ForceMode.Impulse);
        //rb.AddForce(worldVelocity * teleportStrength * 100f);
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
        /*
        if (other.gameObject.TryGetComponent(out STATS stats))
        {
            if (stats.canBeTeleported && !IgnoredParentObjects.shouldBeIgnored(stats.transform))
                StartCoroutine(NewTeleport(stats.gameObject));
        }
        else if (other.gameObject.TryGetComponent(out ICustomTeleport customTeleport))
        {
            StartCoroutine(NewTeleport(customTeleport.ReturnGameobject()));
        }
        */

        if (other.isTrigger) return;

        if (other.gameObject.TryGetComponent(out ICustomTeleport customTeleport))
            StartCoroutine(NewTeleport(customTeleport.ReturnGameobject()));
        else if (other.gameObject.TryGetComponent(out Rigidbody rb)) StartCoroutine(NewTeleport(rb.gameObject));
    }


    public void PlayParticles()
    {
        DOVirtual.DelayedCall(0.2f, () => { teleportParticles.Emit(25); });
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