using System;
using System.Collections.Generic;
using DG.Tweening;
using SmartData.SmartEvent;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class HoldableItem
{
    public Transform itemTransform;
    public bool isThrowable;
}

public class PlayerInteractionHandler : MonoBehaviour, IPressurePlateListener
{
    private static readonly int GlowColor = Shader.PropertyToID("_GlowColor");
    private static readonly int GlowState = Shader.PropertyToID("_GlowState");
    [SerializeField] private PlayerMovement pM;
    [SerializeField] private PlayerCamera pC;
    [SerializeField] private PlayerSoundManager pS;
    [SerializeField] private GameObject playerPickupArea;

    [SerializeField] private STATS stats;
    [SerializeField] private EventDispatcher onPlayerHealthChanged;
    [SerializeField] private SkinnedMeshRenderer playerMesh;

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

    private void Start()
    {
        instance = this;
        var obj = Instantiate(playerPickupArea);
        var pickupArea = obj.GetComponent<PlayerPickupArea>();

        pickupArea.player = gameObject.transform;
        pickupArea.pI = this;
        pickupArea.pM = pM;

        playerReference.playerGameObject = gameObject;
        playerReference.playerCamera = pC;
        playerReference.playerInteractionHandler = this;
        playerReference.playerMovement = pM;
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
                    transform.position + Vector3.up * 0.75f + xPos * pM.isFacingRight, Time.deltaTime * 10);
            }
            else
            {
                holdingItems[i].itemTransform.position = Vector3.Lerp(holdingItems[i].itemTransform.position,
                    transform.position + Vector3.up * 1.3f, Time.deltaTime * 10);
            }
    }


    private void OnDisable()
    {
        DOTween.Kill(transform);
        DOTween.Kill(pM.my3DModel.transform);
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
                if (pM.frozen > 0) return;

                pM.frozen = 15f;
                pM.my3DModel.transform.DOShakePosition(5f, .3f);
                pM.my3DModel.transform.DOLocalRotate(new Vector3(0, 0, 360), .3f, RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);


                pM.DisablePlayer();
                transform.DOMove(collision.transform.position, 1f).SetEase(Ease.InQuart).onComplete += () =>
                {
                    transform.DOScale(Vector3.zero, .5f).SetEase(Ease.InQuart).onComplete += () =>
                    {
                        stats.transitionType.value = (int)LevelLoader.LevelTransitionState.DontLoadYet;
                        stats.transitionEvent.Dispatch();
                    };
                };
                break;
            case "Wind":
                var wind = collision.GetComponent<WindFxScript>();
                pM.windForceTo += wind.moveDir * wind.force * 15f;
                break;
            case "CameraZone":
                var camZone = collision.GetComponent<CameraZone>();
                pC.UpdateCameraZone(camZone);
                break;
            case "Teleport":
                var teleport = collision.GetComponent<TeleporterScript>();
                teleport.Teleport(gameObject);
                GlowPlayer(_portalColor, 0.8f);
                break;
            case "Checkpoint":
                var checkpoint = collision.GetComponent<CheckpointScript>();
                if (checkpoint.isActivated == false)
                    if (pM.IncreaseCheckpoint(checkpoint.checkpointNumber))
                        checkpoint.isActivated = true;
                break;
            case "Traveler":
                var traveler = collision.GetComponent<Traveler>();
                traveler.StartTravel(transform, (exitsToTheLeft) =>
                {
                    pM.EnablePlayer();
                    pM.my3DModel.transform.DOKill();
                    if (!exitsToTheLeft)
                        pM.my3DModel.transform.DOLocalRotate(new Vector3(0, 0, 0), .3f, RotateMode.FastBeyond360);
                    else
                        pM.my3DModel.transform.DOLocalRotate(new Vector3(0, 180, 0), .3f, RotateMode.FastBeyond360);

                    stats.ST_Invincibility = false;
                });


                stats.ST_Invincibility = true;
                if (pM.isFacingRight == 1)
                    pM.my3DModel.transform.DOLocalRotate(new Vector3(0, 0, -360), .3f, RotateMode.FastBeyond360)
                        .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
                else
                    pM.my3DModel.transform.DOLocalRotate(new Vector3(0, 0, -360), .3f, RotateMode.FastBeyond360)
                        .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);

                pM.DisablePlayer();

                break;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CameraZone"))
            pC.UpdateCameraZone(null);
    }

    public void GlowPlayer(Color color)
    {
        playerMesh.material.SetFloat(GlowState, 1f);
        playerMesh.material.SetColor(GlowColor, color);
        playerMesh.material.DOFloat(0, GlowState, 0.2f).SetDelay(0.1f);
    }

    public void GlowPlayer(Color color, float duration)
    {
        playerMesh.material.SetFloat(GlowState, 1f);
        playerMesh.material.SetColor(GlowColor, color);
        playerMesh.material.DOFloat(0, GlowState, 0.2f).SetDelay(duration);
    }

    public void GlowPlayer()
    {
        GlowPlayer(Color.green);
    }

    public static void GlowPlayerStatic(Color color)
    {
        instance.GlowPlayer(color);
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
            if (pM.isDashing && _otherStats.ST_MaxHealth != 999) //if i'm dashing and the enemy is not invulnerable
            {
                if (_otherStats.ST_Health - stats.ST_Damage > 0) //if the enemy wont directly die from the attack
                {
                    //pM.inputDirection = Vector2.Reflect(pM.inputDirection, collision.contacts[0].normal);
                    pM.inputDirection = -_reflectDir;
                    pM.hSpeed = pM.inputDirection.x * 1f;
                    pM.vSpeed = pM.inputDirection.y * 1f;


                    var otherShakeStrength = (Math.Abs(pM.hSpeed) + Math.Abs(pM.vSpeed)) * .65f;
                    _otherStats.transform.DOShakeScale(.2f, otherShakeStrength);
                    _otherStats.transform.DOShakePosition(.2f, otherShakeStrength);
                }
                else //if the enemy dies
                {
                    pM.frozen = .1f;
                    LevelManager.OnScoreChanged?.Invoke(_otherStats.ST_Reward);
                }

                _otherStats.TakeDamage(stats.ST_Damage, transform.position, false);

                ScreenFXSystem.DistortView(0.3f);
                pC.closeUpOffset = .35f;
                pC.closeUpOffsetTo = 1f;
            }
            else
            {
                if (stats.ST_Invincibility == false && _otherStats.ST_Damage > 0 &&
                    _otherStats.ST_CanDoDmg) //if im not dashing and the enemy can attack
                {
                    pM.inputDirection = -_reflectDir;
                    pM.hSpeed = pM.inputDirection.x * 5f;
                    pM.vSpeed = pM.inputDirection.y * 5f;
                    pM.inputDirectionTo = pM.inputDirection;


                    stats.TakeDamage(_otherStats.ST_Damage, _otherStats.transform.position, false);

                    holdingItems.Clear();
                    onPlayerHealthChanged.Dispatch();

                    ScreenFXSystem.FreezeFrames(.3f);
                    ScreenFXSystem.DistortView(0.3f);
                    pM.frozen = .08f;
                    pC.closeUpOffset = .35f;
                    pC.closeUpOffsetTo = 1f;

                    var spikeBall = collision.gameObject.GetComponent<SpikeBallEnemy>();
                    if (spikeBall != null)
                        spikeBall.GotHit();
                }
            }
        }
        else if (_otherStats == null && !stats.IsInsideElevator)
        {
            //if (pM.ReturnVelocity().magnitude < 3f || pM.lastBumpTime > 0) return;

            if (pM.lastBumpTime > 0) return;
            pM.lastBumpTime = .5f;

            transform.DOShakeScale(.2f, (Math.Abs(pM.hSpeed) + Math.Abs(pM.vSpeed)) * .65f).onComplete +=
                () => transform.DOScale(Vector3.one, .2f);

            EffectHandler.SpawnFX((int)EffectHandler.EffectType.Clash, collision.contacts[0].point, Vector3.zero,
                Vector3.zero, 0);

            pS.PlayWallHitSound();


            pM.inputDirection = Vector2.Reflect(pM.inputDirection, collision.contacts[0].normal);
            //pM.inputDirection = -_reflectDir;
            pM.hSpeed = pM.inputDirection.x * 1.5f;
            pM.vSpeed = pM.inputDirection.y * 1.5f;

            if (collision.gameObject.CompareTag("DSwitcher") && pM.isDashing)
                collision.gameObject.GetComponent<DSwitcherScript>().Hit(transform.position);

            else if (collision.gameObject.CompareTag("Button") && pM.isDashing)
                collision.gameObject.GetComponent<ButtonScript>().Press();
        }
    }

    public void CheckTakeDamage(int damage, Vector3 damagePos)
    {
        if (stats.ST_Invincibility == false) //if im not dashing and the enemy can attack
        {
            print("check take damage");
            _reflectDir = Vector3.Reflect(damagePos, transform.position).normalized;
            pM.inputDirection = -_reflectDir;
            pM.hSpeed = pM.inputDirection.x * 5f;
            pM.vSpeed = pM.inputDirection.y * 5f;
            pM.inputDirectionTo = pM.inputDirection;

            stats.TakeDamage(damage, damagePos, false);

            holdingItems.Clear();
            onPlayerHealthChanged.Dispatch();

            ScreenFXSystem.FreezeFrames(.3f);
            ScreenFXSystem.DistortView(0.3f);
            pM.frozen = .08f;
            pC.closeUpOffset = .35f;
            pC.closeUpOffsetTo = 1f;
        }
    }

    public void QuitGame(InputAction.CallbackContext context)
    {
        if (context.performed)
            Application.Quit();
    }
}