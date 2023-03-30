using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckEnemyDamage : MonoBehaviour
{
    [SerializeField] private STATS myStats;

    private void OnCollisionEnter(Collision collision)
    {
        var otherStats = collision.gameObject.GetComponent<STATS>();
        if (otherStats != null)
            if (otherStats.ST_Damage > 0 && otherStats.ST_Team == STATS.Team.Neutral)
                myStats.TakeDamage(otherStats.ST_Damage, otherStats.dmgDirection);
    }
}