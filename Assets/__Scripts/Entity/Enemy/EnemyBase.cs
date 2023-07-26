using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    private EnemyAI _enemyAI;
    
    public Stats stats = Stats.CreateInstance();

    private void Start()
    {
        _enemyAI = GetComponent<EnemyAI>();
        _enemyAI.Initialize(this);
    }
    
    public void Attacked(int p_pDamage, Player p_pAttacker)
    {
        stats.Health -= p_pDamage;
        Knockback(p_pAttacker, 10f);
        if (stats.Health <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    public void Knockback(Player p_pAttacker, float p_pKnockbackForce)
    {
        Vector2 _knockbackDirection = (transform.position - p_pAttacker.PlayerPosition).normalized;
        GetComponent<Rigidbody2D>().AddForce(_knockbackDirection * p_pKnockbackForce, ForceMode2D.Impulse);
    }
}
