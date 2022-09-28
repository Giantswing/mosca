using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInteractionHandler : MonoBehaviour
{
    [SerializeField] private PlayerMovement pM;
    [SerializeField] private PlayerCamera pC;
    [SerializeField] private STATS stats;

    public static UnityAction<int, int> OnPlayerHealthChanged;

    private Vector3 _vertSqueeze = new(0, 0.5f, 0);
    private Vector3 _horSqueeze = new(0.5f, 0, 0);
    private Vector3 _squeeze;


    private void Start()
    {
        StartCoroutine(StartHealth());
    }

    private IEnumerator StartHealth()
    {
        yield return new WaitForSeconds(.2f);
        OnPlayerHealthChanged?.Invoke(stats.ST_Health, stats.ST_MaxHealth);
    }

    private void OnCollisionEnter(Collision collision)
    {
        transform.DOShakeScale(.2f, (Math.Abs(pM.hSpeed) + Math.Abs(pM.vSpeed)) * .65f).onComplete +=
            () => transform.DOScale(Vector3.one, .2f);

        /*
        _squeeze = Vector3.one - Vector3.right * .5f;
        transform.DOScale(_squeeze, .1f).SetEase(Ease.Linear)
            .OnComplete(() => transform.DOScale(Vector3.one, .1f).SetEase(Ease.OutBounce));
        */

        var otherStats = collision.gameObject.GetComponent<STATS>();
        if (otherStats != null && otherStats.ST_Invincibility == false && otherStats.ST_Team != stats.ST_Team)
        {
            if (pM.isDashing && otherStats.ST_MaxHealth != 999)
            {
                if (otherStats.ST_Health - stats.ST_Damage > 0)
                {
                    pM.inputDirection = Vector2.Reflect(pM.inputDirection, collision.contacts[0].normal);
                    pM.hSpeed = pM.inputDirection.x * 1f;
                    pM.vSpeed = pM.inputDirection.y * 1f;

                    var otherShakeStrength = (Math.Abs(pM.hSpeed) + Math.Abs(pM.vSpeed)) * .65f;
                    otherStats.transform.DOShakeScale(.2f, otherShakeStrength);
                    otherStats.transform.DOShakePosition(.2f, otherShakeStrength);
                }
                else
                {
                    pM.frozen = .1f;
                    LevelManager.OnScoreChanged?.Invoke(otherStats.ST_Reward);
                }

                otherStats.TakeDamage(stats.ST_Damage);

                FreezeFrameScript.DistortView(0.3f);
                pC.closeUpOffset = .35f;
                pC.closeUpOffsetTo = 1f;
            }
            else
            {
                if (otherStats.ST_Damage > 0)
                {
                    pM.inputDirection = Vector2.Reflect(pM.inputDirection, collision.contacts[0].normal);
                    pM.hSpeed = pM.inputDirection.x * 2f;
                    pM.vSpeed = pM.inputDirection.y * 2f;
                    pM.inputDirectionTo = pM.inputDirection;


                    stats.TakeDamage(otherStats.ST_Damage);
                    OnPlayerHealthChanged?.Invoke(stats.ST_Health, stats.ST_MaxHealth);

                    FreezeFrameScript.FreezeFrames(0.3f);
                    FreezeFrameScript.DistortView(0.3f);
                    pM.frozen = .08f;
                    pC.closeUpOffset = .35f;
                    pC.closeUpOffsetTo = 1f;

                    var spikeBall = collision.gameObject.GetComponent<SpikeBallEnemy>();
                    if (spikeBall != null)
                        spikeBall.GotHit();
                }
            }
        }
        else if (otherStats == null)
        {
            if (pM.inputDirection.magnitude <= 0.85f) return;

            EffectHandler.SpawnFX((int)EffectHandler.EffectType.Clash, collision.contacts[0].point, Vector3.zero,
                Vector3.zero, 0);


            pM.inputDirection = Vector2.Reflect(pM.inputDirection, collision.contacts[0].normal);
            pM.hSpeed = pM.inputDirection.x * 1.5f;
            pM.vSpeed = pM.inputDirection.y * 1.5f;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Collectable"))
        {
            var collectable = collision.gameObject.GetComponent<CollectableBehaviour>();

            if (collectable != null && collectable.isFollowing == 0)
                collectable.Collect(transform);
        }

        if (collision.gameObject.CompareTag("Meta"))
        {
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
                    LevelManager.StartLevelTransition?.Invoke(collision.transform.position);
                };
            };
        }
    }

    private void OnDisable()
    {
        DOTween.Kill(transform);
        DOTween.Kill(pM.my3DModel.transform);
    }
}