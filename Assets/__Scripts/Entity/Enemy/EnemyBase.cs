using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    private EnemyAI          _enemyAI;
    public  MMF_Player       _damageFeedback;
    public  Stats            stats = Stats.CreateInstance();
    public  MMF_FloatingText _floatingText;

    private void Start()
    {
        _enemyAI = GetComponent<EnemyAI>();
        _enemyAI.Initialize(this);
        _damageFeedback = transform.Find("DamageFeedback").GetComponent<MMF_Player>();
        _floatingText   = _damageFeedback.GetFeedbackOfType<MMF_FloatingText>();
    }

    public void Attacked(int p_pDamage, float p_stunDuration, Player p_pAttacker)
    {
        stats.health -= p_pDamage;

        _floatingText.Value = p_pDamage.ToString();
        _damageFeedback.PlayFeedbacks();

        if (p_stunDuration == 0)
            _enemyAI.Daze();
        else
            _enemyAI.Stun(p_stunDuration);
        
        Knockback(p_pAttacker, 100);
        if (stats.health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void Knockback(Player p_pAttacker, float p_pKnockbackForce)
    {
        Vector2 _knockbackDirection = (transform.position - p_pAttacker.PlayerPosition).normalized;
        // GetComponent<Rigidbody2D>().velocity = _knockbackDirection * p_pKnockbackForce;
        GetComponent<Rigidbody2D>().AddForce(_knockbackDirection * p_pKnockbackForce, ForceMode2D.Impulse);
    }
}