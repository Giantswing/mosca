using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;


public class STATS : MonoBehaviour
{
    [Header("Sound")] [SerializeField] private SimpleAudioEvent deathSoundEvent;
    [SerializeField] private bool playDeathSound = false;

    [Space(5)] [SerializeField] private SimpleAudioEvent hitSoundEvent;

    [SerializeField] private bool playHitSound = false;

    [Space(10)] public Renderer myRenderer;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material invincibilityMaterial;

    public bool IsInsideElevator = false;

    public enum Team
    {
        Neutral,
        Player,
        Enemy
    }


    public float ST_Speed;
    public float ST_SpeedBoost;
    public SmartData.SmartInt.IntWriter ST_Health;
    public int ST_MaxHealth;

    public int ST_Damage;
    public Team ST_Team; //0 => Neutral, 1 => Player, 2 => Enemy

    public bool ST_Invincibility;
    public bool ST_CanDoDmg = false;
    [SerializeField] private float ST_InvincibilityTimer;
    public int ST_Reward;

    [SerializeField] private bool _hasHitEvent = false;
    public UnityEvent ST_HitEvent;

    public UnityEvent ST_DeathEvent;

    [SerializeField] private bool onlyDamageByExplosions = false;
    [SerializeField] private bool isThisAnExplosion = false;
    public bool isPlayer = false;
    public bool canBeTeleported = false;


    private bool HasInvincibilityMaterial = false;

    [HideInInspector] public Vector3 dmgDirection;

    private bool isAlive = true;


    /* TWEENS */
    private Tweener _speedBoostTween;

    private void Start()
    {
        //ST_MaxHealth = ST_Health;


        if (ST_InvincibilityTimer > 0)
            if (myRenderer != null)
            {
                normalMaterial = myRenderer.sharedMaterial;
                HasInvincibilityMaterial = true;
            }
    }

    public void ChangeSpeedBoost(float newSpeedBoost, float changeSpeed = 0, float delay = 0)
    {
        if (changeSpeed == 0)
        {
            _speedBoostTween?.Kill();
            ST_SpeedBoost = newSpeedBoost;
        }
        else
        {
            _speedBoostTween =
                DOTween.To(() => ST_SpeedBoost, x => ST_SpeedBoost = x, newSpeedBoost, changeSpeed);
        }
    }

    public void SimpleDeath()
    {
        Destroy(gameObject);
    }

    public void SimpleDeathDelay()
    {
        StartCoroutine(IE_SimpleDeathDelay());
    }

    private IEnumerator IE_SimpleDeathDelay()
    {
        GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    public void TakeDamage(int dmg, Vector3 originDmgPos, bool isExplosion = false)
    {
        if (ST_Invincibility) return;
        if (!isExplosion && onlyDamageByExplosions) return;

        if (playHitSound)
            GlobalAudioManager.PlaySound(hitSoundEvent, transform.position);

        ST_Health.value -= dmg;
        dmgDirection = originDmgPos;

        if (ST_Health <= 0 && isAlive)
        {
            ST_CanDoDmg = false;
            ST_Damage = 0;

            SaveLoadSystem.SaveGame();
            if (ST_Team == Team.Player)
            {
                LevelManager.GetCurrentLevel().deathCounter++;
                DeathCounterScript.UpdateDeathCounter();
            }

            Die();
            isAlive = false;
        }
        else
        {
            if (_hasHitEvent)
                ST_HitEvent.Invoke();
            StartCoroutine(MakeInvincible());
            if (HasInvincibilityMaterial)
                StartCoroutine(InvincibleEffect());
        }

        if (ST_Team == Team.Player) EffectHandler.SpawnFX(4, transform.position, Vector3.zero, Vector3.zero, 0);
    }

    private IEnumerator InvincibleEffect()
    {
        var repeat = 8;

        for (var i = 0; i < repeat; i++)
        {
            myRenderer.sharedMaterial = invincibilityMaterial;
            yield return new WaitForSeconds(ST_InvincibilityTimer / repeat);
            myRenderer.sharedMaterial = normalMaterial;
            yield return new WaitForSeconds(ST_InvincibilityTimer / repeat);
        }
    }

    private IEnumerator MakeInvincible()
    {
        ST_Invincibility = true;
        yield return new WaitForSeconds(ST_InvincibilityTimer);
        ST_Invincibility = false;
    }

    private void Die()
    {
        /*
        if (ST_Team == Team.Player && isAlive)
        {
            //transitionType.value = (int)LevelLoader.LevelTransitionState.Restart;
            //transitionEvent.Dispatch();
        }
        else
        {
            ST_DeathEvent?.Invoke();
            if (playDeathSound)
                GlobalAudioManager.PlaySound(deathSoundEvent, transform.position);
        }*/

        ST_DeathEvent?.Invoke();
        if (playDeathSound)
            GlobalAudioManager.PlaySound(deathSoundEvent, transform.position);
    }
}