using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class STATS : MonoBehaviour
{
    [Header("Sound")] [SerializeField] private SimpleAudioEvent deathSoundEvent;
    [SerializeField] private bool playDeathSound = false;

    [Space(5)] [Header("Sound")] [SerializeField]
    private SimpleAudioEvent hitSoundEvent;

    [SerializeField] private bool playHitSound = false;

    [Space(10)] [SerializeField] private Renderer _renderer;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material invincibilityMaterial;


    public float ST_Speed;
    public SmartData.SmartInt.IntWriter ST_Health;
    public int ST_MaxHealth;

    public int ST_Damage;
    public int ST_Team; //0 => Neutral, 1 => Player, 2 => Enemy

    public bool ST_Invincibility;
    public bool ST_CanDoDmg = false;
    [SerializeField] private float ST_InvincibilityTimer;
    public int ST_Reward;

    [SerializeField] private bool _hasHitEvent = false;
    public UnityEvent ST_HitEvent;

    public UnityEvent ST_DeathEvent;


    private bool HasInvincibilityMaterial = false;

    [HideInInspector] public Vector3 dmgDirection;

    public SmartData.SmartEvent.EventDispatcher transitionEvent;
    public SmartData.SmartInt.IntWriter transitionType;

    private bool isAlive = true;

    private void Start()
    {
        //ST_MaxHealth = ST_Health;


        if (ST_InvincibilityTimer > 0)
        {
            normalMaterial = _renderer.sharedMaterial;
            HasInvincibilityMaterial = true;
        }
    }

    public void TakeDamage(int dmg, Vector3 originDmgPos)
    {
        if (ST_Invincibility) return;

        if (playHitSound)
            GlobalAudioManager.PlaySound(hitSoundEvent, transform.position);

        ST_Health.value -= dmg;
        dmgDirection = originDmgPos;

        if (ST_Health <= 0 && isAlive)
        {
            ST_CanDoDmg = false;
            ST_Damage = 0;
            CurrentLevelHolder.GetCurrentLevel().deathCounter++;
            SaveLoadSystem.SaveGame();
            if (ST_Team == 1)
                DeathCounterScript.UpdateDeathCounter();
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

        if (ST_Team == 1) EffectHandler.SpawnFX(4, transform.position, Vector3.zero, Vector3.zero, 0);
    }

    private IEnumerator InvincibleEffect()
    {
        var repeat = 8;

        for (var i = 0; i < repeat; i++)
        {
            _renderer.sharedMaterial = invincibilityMaterial;
            yield return new WaitForSeconds(ST_InvincibilityTimer / repeat);
            _renderer.sharedMaterial = normalMaterial;
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
        if (ST_Team == 1 && isAlive)
        {
            transitionType.value = (int)LevelLoader.LevelTransitionState.Restart;
            transitionEvent.Dispatch();
        }
        else
        {
            ST_DeathEvent?.Invoke();
            if (playDeathSound)
                GlobalAudioManager.PlaySound(deathSoundEvent, transform.position);
        }
    }
}