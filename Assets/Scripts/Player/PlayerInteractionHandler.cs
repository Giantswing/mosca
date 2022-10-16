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

    private Vector3 _vertSqueeze = new(0, 0.5f, 0);
    private Vector3 _horSqueeze = new(0.5f, 0, 0);
    private Vector3 _squeeze;

    private STATS _otherStats;
    private Vector3 _reflectDir;

    [SerializeField] private Collider dashCollider;

    private void OnCollisionEnter(Collision collision)
    {
        transform.DOShakeScale(.2f, (Math.Abs(pM.hSpeed) + Math.Abs(pM.vSpeed)) * .65f).onComplete +=
            () => transform.DOScale(Vector3.one, .2f);

        CheckDMG(collision);

        if (dashCollider.enabled)
            dashCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Collectable"))
        {
            var collectable = collision.gameObject.GetComponent<CollectableBehaviour>();

            if (collectable != null && collectable.isFollowing == 0)
                collectable.Collect(transform);
        }

        else if (collision.gameObject.CompareTag("Meta"))
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
                    LevelManager.StartLevelTransition?.Invoke((int)LevelManager.LevelTransitionState.NextLevel,
                        null);
                };
            };
        }

        else if (collision.gameObject.CompareTag("SecretWall"))
        {
            var other = collision.GetComponent<SecretWallScript>();
            other.Disappear();
        }
    }

    private void CheckDMG(Collision collision)
    {
        _reflectDir = (collision.transform.position - transform.position).normalized;
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

                _otherStats.TakeDamage(stats.ST_Damage, transform.position);

                FreezeFrameScript.DistortView(0.3f);
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


                    stats.TakeDamage(_otherStats.ST_Damage, _otherStats.transform.position);

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
        else if (_otherStats == null)
        {
            if (pM.inputDirection.magnitude <= 0.85f) return;

            EffectHandler.SpawnFX((int)EffectHandler.EffectType.Clash, collision.contacts[0].point, Vector3.zero,
                Vector3.zero, 0);


            pM.inputDirection = Vector2.Reflect(pM.inputDirection, collision.contacts[0].normal);
            //pM.inputDirection = -_reflectDir;
            pM.hSpeed = pM.inputDirection.x * 1.5f;
            pM.vSpeed = pM.inputDirection.y * 1.5f;
        }
    }


    private void OnDisable()
    {
        DOTween.Kill(transform);
        DOTween.Kill(pM.my3DModel.transform);
    }
}