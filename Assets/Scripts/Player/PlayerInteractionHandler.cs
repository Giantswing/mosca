using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInteractionHandler : MonoBehaviour
{
    [SerializeField] private PlayerMovement pM;
    [SerializeField] private PlayerCamera pC;
    [SerializeField] private STATS stats;

    public static UnityAction<int, int> OnPlayerHealthChange;
    public static UnityAction<int> OnScoreChange;

    private Vector3 _vertSqueeze = new(0, 0.5f, 0);
    private Vector3 _horSqueeze = new(0.5f, 0, 0);
    private Vector3 _squeeze;


    private void Start()
    {
        OnPlayerHealthChange?.Invoke(stats.ST_Health, stats.ST_MaxHealth);
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
            if (pM.isDashing)
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
                    OnScoreChange?.Invoke(otherStats.ST_Reward);
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


                    stats.TakeDamage(otherStats.ST_Damage);
                    OnPlayerHealthChange?.Invoke(stats.ST_Health, stats.ST_MaxHealth);

                    FreezeFrameScript.FreezeFrames(0.3f);
                    FreezeFrameScript.DistortView(0.3f);
                    pM.frozen = .08f;
                    pC.closeUpOffset = .35f;
                    pC.closeUpOffsetTo = 1f;
                }
            }
        }
        else if (otherStats == null)
        {
            pM.inputDirection = Vector2.Reflect(pM.inputDirection, collision.contacts[0].normal);
            pM.hSpeed = pM.inputDirection.x * .5f;
            pM.vSpeed = pM.inputDirection.y * .5f;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            var coin = collision.gameObject.GetComponent<CoinScript>();

            if (coin != null && coin.isFollowing == 0)
                coin.Collect(transform);
        }
    }
}