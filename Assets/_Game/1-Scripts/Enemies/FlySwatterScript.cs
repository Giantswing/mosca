using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlySwatterScript : MonoBehaviour
{
    [SerializeField] private Attributes attributes;
    [SerializeField] private Collider dmgCollider;
    [SerializeField] private Animator _animator;
    [SerializeField] private SimpleAudioEvent hitSound;

    private bool _isAttacking = false;
    private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
    private readonly WaitForSeconds _coolDownTime = new(.6f);

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
        SoundMaster.PlaySound(transform.position, (int)SoundListAuto.FlySwatterHit, true);
        dmgCollider.enabled = true;
        attributes.canDoDamage = true;
        FXMaster.SpawnFX(dmgCollider.bounds.center, (int)FXListAuto.Clash);
    }

    public void EndDMG()
    {
        dmgCollider.enabled = false;
        attributes.canDoDamage = false;
    }

    public void EndAttack()
    {
        _animator.SetBool(IsAttacking, false);
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        yield return _coolDownTime;
        _isAttacking = false;
    }
}