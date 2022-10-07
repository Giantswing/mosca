using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class STATS : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material invincibilityMaterial;


    public float ST_Speed;
    public int ST_Health;
    public int ST_MaxHealth;

    public int ST_Damage;
    public int ST_Team; //0 => Neutral, 1 => Player, 2 => Enemy

    public bool ST_Invincibility;
    public bool ST_CanDoDmg = false;
    [SerializeField] private float ST_InvincibilityTimer;
    public int ST_Reward;
    public UnityEvent ST_DeathEvent;


    private bool HasInvincibilityMaterial = false;

    private void Start()
    {
        ST_MaxHealth = ST_Health;


        if (ST_InvincibilityTimer > 0)
        {
            normalMaterial = _renderer.sharedMaterial;
            HasInvincibilityMaterial = true;
        }
    }

    public void TakeDamage(int dmg)
    {
        ST_Health -= dmg;
        //print(string.Concat("GameObject ", gameObject.name, " took ", dmg, " damage"));

        if (ST_Health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(MakeInvincible());
            if (HasInvincibilityMaterial)
                StartCoroutine(InvincibleEffect());
        }
    }

    private IEnumerator InvincibleEffect()
    {
        var repeat = 4;

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
        if (ST_Team == 1)
        {
            LevelManager.RestartLevel();
        }
        else
        {
            ST_DeathEvent?.Invoke();
            Destroy(gameObject);
        }
    }
}