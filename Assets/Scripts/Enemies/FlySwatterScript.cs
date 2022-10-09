using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlySwatterScript : MonoBehaviour
{
    [SerializeField] private STATS stats;
    [SerializeField] private Collider dmgCollider;
    [SerializeField] private Animator _animator;
    private bool _isAttacking = false;
    private static readonly int IsAttacking = Animator.StringToHash("isAttacking");

    private void Start()
    {
        dmgCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isAttacking)
            if (other.CompareTag("Player"))
            {
                _isAttacking = true;
                _animator.SetBool(IsAttacking, _isAttacking);
            }
    }

    public void StartDMG()
    {
        dmgCollider.enabled = true;
        stats.ST_CanDoDmg = true;
        EffectHandler.SpawnFX((int)EffectHandler.EffectType.Clash, dmgCollider.bounds.center, Vector3.zero,
            Vector3.zero, 0);
    }

    public void EndDMG()
    {
        dmgCollider.enabled = false;
        stats.ST_CanDoDmg = false;
    }

    public void EndAttack()
    {
        _isAttacking = false;
        _animator.SetBool(IsAttacking, _isAttacking);
    }
}