using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class STATS : MonoBehaviour
{
    public float ST_Speed;
    public int ST_Health;
    public int ST_MaxHealth;

    public int ST_Damage;
    public int ST_Team; //0 => Neutral, 1 => Player, 2 => Enemy

    public bool ST_Invincibility;
    [SerializeField] private int ST_InvincibilityTimer;
    public int ST_Reward;
    public UnityEvent ST_DeathEvent;

    private void Start()
    {
        ST_MaxHealth = ST_Health;
    }

    public void TakeDamage(int dmg)
    {
        ST_Health -= dmg;
        //print(string.Concat("GameObject ", gameObject.name, " took ", dmg, " damage"));

        if (ST_Health <= 0) Die();
        else
            StartCoroutine(MakeInvincible());
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
            GameManagerScript.RestartLevel();
        }
        else
        {
            ST_DeathEvent?.Invoke();
            Destroy(gameObject);
        }
    }
}