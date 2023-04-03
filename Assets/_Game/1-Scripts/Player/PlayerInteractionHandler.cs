using System;
using System.Collections.Generic;
using DG.Tweening;
using SmartData.SmartEvent;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[Serializable]
public class HoldableItem
{
    public Transform itemTransform;
    public bool isThrowable;
}

public class PlayerInteractionHandler : MonoBehaviour, IPressurePlateListener
{
    /* REFERENCES */
    private PlayerMovement playerMovement;
    private PlayerCamera playerCamera;
    private PlayerSoundManager playerSoundManager;
    [SerializeField] private GameObject playerPickupArea;
    private Dash dash;
    private GlowHandler glowHandler;
    private STATS stats;

    /* ----------------- */


    [SerializeField] private EventDispatcher onPlayerHealthChanged;


    [SerializeField] private Collider dashCollider;
    [SerializeField] private Collider damageCollider;
    [SerializeField] private float lastBumpTime;

    [Space(25)] public List<HoldableItem> holdingItems;
    private readonly Color _portalColor = new(0.45f, 0.15f, 0.5f);
    private Vector3 _horSqueeze = new(0.5f, 0, 0);
    public static PlayerInteractionHandler instance;

    private STATS _otherStats;
    private Vector3 _reflectDir;
    private Vector3 _squeeze;

    private Vector3 _vertSqueeze = new(0, 0.5f, 0);

    [SerializeField] private PlayerReferenceSO playerReference;

    public EventDispatcher transitionEvent;
    public SmartData.SmartInt.IntWriter transitionType;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerCamera = GetComponentInChildren<PlayerCamera>();
        playerSoundManager = GetComponent<PlayerSoundManager>();
        dash = GetComponent<Dash>();
        stats = GetComponent<STATS>();
    }

    private void Start()
    {
        instance = this;
        var obj = Instantiate(playerPickupArea);
        var pickupArea = obj.GetComponent<PlayerPickupArea>();

        pickupArea.player = gameObject.transform;
        pickupArea.pI = this;
        pickupArea.pM = playerMovement;

        playerReference.playerGameObject = gameObject;
        playerReference.playerCamera = playerCamera;
        playerReference.playerInteractionHandler = this;
        playerReference.playerMovement = playerMovement;
        playerReference.playerRigidbody = GetComponent<Rigidbody>();
        playerReference.playerTransform = transform;
    }

    private void Update()
    {
        if (holdingItems.Count == 0) return;

        for (var i = 0; i < holdingItems.Count; i++)
            if (!holdingItems[i].isThrowable)
            {
                var xPos = Vector3.left * 1.5f + Vector3.right * (0.75f * i);
                holdingItems[i].itemTransform.position = Vector3.Lerp(holdingItems[i].itemTransform.position,
                    transform.position + Vector3.up * 0.75f + xPos * playerMovement.isFacingRight, Time.deltaTime * 10);
            }
            else
            {
                holdingItems[i].itemTransform.position = Vector3.Lerp(holdingItems[i].itemTransform.position,
                    transform.position + Vector3.up * 1.3f, Time.deltaTime * 10);
            }
    }

    public bool HasThrowableItem()
    {
        for (var i = 0; i < holdingItems.Count; i++)
            if (holdingItems[i].isThrowable)
                return true;

        return false;
    }

    private void OnDisable()
    {
        DOTween.Kill(transform);
        DOTween.Kill(playerMovement.my3DModel.transform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckDmg(collision);

        if (dashCollider.enabled)
            dashCollider.enabled = false;
    }


    private void OnTriggerEnter(Collider collision)
    {
        //cache collision tag
        var tag = collision.tag;

        switch (tag)
        {
            case "Meta":
                if (playerMovement.frozen > 0) return;

                playerMovement.frozen = 15f;
                playerMovement.my3DModel.transform.DOShakePosition(5f, .3f);
                playerMovement.my3DModel.transform.DOLocalRotate(new Vector3(0, 0, 360), .3f, RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);


                playerMovement.DisablePlayer();
                transform.DOMove(collision.transform.position, 1f).SetEase(Ease.InQuart).onComplete += () =>
                {
                    transform.DOScale(Vector3.zero, .5f).SetEase(Ease.InQuart).onComplete += () =>
                    {
                        transitionType.value = (int)LevelLoader.LevelTransitionState.DontLoadYet;
                        transitionEvent.Dispatch();
                    };
                };
                break;
            case "Wind":
                var wind = collision.GetComponent<WindFxScript>();
                playerMovement.windForceTo += wind.moveDir * wind.force * 15f;
                break;
            case "CameraZone":
                var camZone = collision.GetComponent<CameraZone>();
                playerCamera.UpdateCameraZone(camZone);
                break;
            /*
            case "Teleport":
                var teleport = collision.GetComponent<TeleporterScript>();
                teleport.Teleport(gameObject);
                glowHandler.Glow(_portalColor, 0.8f);
                break;
                */
            case "Traveler":
                var traveler = collision.GetComponent<Traveler>();
                traveler.StartTravel(transform, (exitsToTheLeft) =>
                {
                    playerMovement.EnablePlayer();
                    playerMovement.my3DModel.transform.DOKill();
                    if (!exitsToTheLeft)
                        playerMovement.my3DModel.transform.DOLocalRotate(new Vector3(0, 0, 0), .3f,
                            RotateMode.FastBeyond360);
                    else
                        playerMovement.my3DModel.transform.DOLocalRotate(new Vector3(0, 180, 0), .3f,
                            RotateMode.FastBeyond360);

                    stats.ST_Invincibility = false;
                });


                stats.ST_Invincibility = true;
                if (playerMovement.isFacingRight == 1)
                    playerMovement.my3DModel.transform
                        .DOLocalRotate(new Vector3(0, 0, -360), .3f, RotateMode.FastBeyond360)
                        .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
                else
                    playerMovement.my3DModel.transform
                        .DOLocalRotate(new Vector3(0, 0, -360), .3f, RotateMode.FastBeyond360)
                        .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);

                playerMovement.DisablePlayer();

                break;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CameraZone"))
            playerCamera.UpdateCameraZone(null);
    }

    public void Die()
    {
        transitionType.value = (int)LevelLoader.LevelTransitionState.Restart;
        transitionEvent.Dispatch();
    }


    public void MakeInvincible(bool invincible)
    {
        if (invincible)
        {
            damageCollider.enabled = false;
            stats.ST_Invincibility = true;
        }
        else
        {
            stats.ST_Invincibility = false;
            damageCollider.enabled = true;
        }
    }

    private void CheckDmg(Collision collision)
    {
        //_reflectDir = (collision.transform.position - transform.position).normalized;
        _reflectDir = Vector3.Reflect(collision.contacts[0].normal, transform.position).normalized;

        _otherStats = collision.gameObject.GetComponent<STATS>();
        if (_otherStats != null && _otherStats.ST_Invincibility == false && _otherStats.ST_Team != stats.ST_Team)
        {
            if (dash.isDashing &&
                _otherStats.ST_MaxHealth != 999) //if i'm dashing and the enemy is not invulnerable
            {
                if (_otherStats.ST_Health - stats.ST_Damage > 0) //if the enemy wont directly die from the attack
                {
                    //pM.inputDirection = Vector2.Reflect(pM.inputDirection, collision.contacts[0].normal);
                    playerMovement.inputDirection = -_reflectDir;
                    playerMovement.hSpeed = playerMovement.inputDirection.x * 1f;
                    playerMovement.vSpeed = playerMovement.inputDirection.y * 1f;


                    var otherShakeStrength = (Math.Abs(playerMovement.hSpeed) + Math.Abs(playerMovement.vSpeed)) * .65f;
                    _otherStats.transform.DOShakeScale(.2f, otherShakeStrength);
                    _otherStats.transform.DOShakePosition(.2f, otherShakeStrength);
                }
                else //if the enemy dies
                {
                    playerMovement.frozen = .1f;
                    LevelManager.OnScoreChanged?.Invoke(_otherStats.ST_Reward);
                }

                _otherStats.TakeDamage(stats.ST_Damage, transform.position, false);

                ScreenFXSystem.DistortView(0.3f);
                playerCamera.closeUpOffset = .35f;
                playerCamera.closeUpOffsetTo = 1f;
            }
            else
            {
                if (stats.ST_Invincibility == false && _otherStats.ST_Damage > 0 &&
                    _otherStats.ST_CanDoDmg) //if im not dashing and the enemy can attack
                {
                    playerMovement.inputDirection = -_reflectDir;
                    playerMovement.hSpeed = playerMovement.inputDirection.x * 5f;
                    playerMovement.vSpeed = playerMovement.inputDirection.y * 5f;
                    playerMovement.inputDirectionTo = playerMovement.inputDirection;


                    stats.TakeDamage(_otherStats.ST_Damage, _otherStats.transform.position, false);

                    holdingItems.Clear();
                    onPlayerHealthChanged.Dispatch();

                    ScreenFXSystem.FreezeFrames(.3f);
                    ScreenFXSystem.DistortView(0.3f);
                    playerMovement.frozen = .08f;
                    playerCamera.closeUpOffset = .35f;
                    playerCamera.closeUpOffsetTo = 1f;

                    var spikeBall = collision.gameObject.GetComponent<SpikeBallEnemy>();
                    if (spikeBall != null)
                        spikeBall.GotHit();
                }
            }
        }
        else if (_otherStats == null && !stats.IsInsideElevator)
        {
            if (collision.gameObject.TryGetComponent(out IGenericInteractable interactable) && dash.isDashing)
            {
                interactable.Interact(transform.position);

                if (lastBumpTime == 0)
                    Bump(collision.contacts[0].normal, collision.contacts[0].point);

                return;
            }


            if (playerMovement.lastBumpTime > 0) return;
            playerMovement.lastBumpTime = .5f;

            playerSoundManager.PlayWallHitSound();

            Bump(collision.contacts[0].normal, collision.contacts[0].point);
        }
    }

    private void Bump(Vector2 normal, Vector3 point)
    {
        transform.DOShakeScale(.2f, (Math.Abs(playerMovement.hSpeed) + Math.Abs(playerMovement.vSpeed)) * .65f)
                .onComplete +=
            () => transform.DOScale(Vector3.one, .2f);

        EffectHandler.SpawnFX((int)EffectHandler.EffectType.Clash, point, Vector3.zero,
            Vector3.zero, 0);


        playerMovement.inputDirection =
            Vector2.Reflect(playerMovement.inputDirection, normal);
        //pM.inputDirection = -_reflectDir;
        playerMovement.hSpeed = playerMovement.inputDirection.x * 1.5f;
        playerMovement.vSpeed = playerMovement.inputDirection.y * 1.5f;

        playerMovement._timeBackwards = 0;
    }

    public void CheckTakeDamage(int damage, Vector3 damagePos)
    {
        if (stats.ST_Invincibility == false) //if im not dashing and the enemy can attack
        {
            print("check take damage");
            _reflectDir = Vector3.Reflect(damagePos, transform.position).normalized;
            playerMovement.inputDirection = -_reflectDir;
            playerMovement.hSpeed = playerMovement.inputDirection.x * 5f;
            playerMovement.vSpeed = playerMovement.inputDirection.y * 5f;
            playerMovement.inputDirectionTo = playerMovement.inputDirection;

            stats.TakeDamage(damage, damagePos, false);

            holdingItems.Clear();
            onPlayerHealthChanged.Dispatch();

            ScreenFXSystem.FreezeFrames(.3f);
            ScreenFXSystem.DistortView(0.3f);
            playerMovement.frozen = .08f;
            playerCamera.closeUpOffset = .35f;
            playerCamera.closeUpOffsetTo = 1f;
        }
    }

    public void QuitGame(InputAction.CallbackContext context)
    {
        if (context.performed)
            Application.Quit();
    }
}