using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    private EnemyAI          _enemyAI;
    private MMF_Player       _damageFeedback;
    private MMF_FloatingText _floatingText;

    public EnemyStats stats;

    private void Start()
    {
        stats = new EnemyStats(GameManager.MonstersData["Frost"]);

        _enemyAI = GetComponent<EnemyAI>();
        _enemyAI.Initialize(this);
        _damageFeedback = transform.Find("DamageFeedback").GetComponent<MMF_Player>();
        _floatingText   = _damageFeedback.GetFeedbackOfType<MMF_FloatingText>();
    }

    private readonly Dictionary<uint, uint> _attackID = new Dictionary<uint, uint>();

    public void Attacked(int p_pDamage, float p_stunDuration, Player p_pAttacker, uint? p_attackID = null)
    {
        _floatingText.Value = p_pDamage.ToString();

        if (!p_attackID.HasValue)
            _floatingText.TargetPosition = transform.position;
        else
        {
            if (_attackID.ContainsKey(p_attackID.Value)) //해당 ID의 공격이 이미 있으면
                _attackID[p_attackID.Value]++;           //해당 아이디의 value를 증가
            else                                         //없으면
                _attackID.Add(p_attackID.Value, 0);      //해당 ID의 공격을 만들고 value를 초기화

            _floatingText.TargetPosition = transform.position + Vector3.up * .7f * _attackID[p_attackID.Value];
        }


        _damageFeedback.PlayFeedbacks();

        if (p_stunDuration == 0)
            _enemyAI.Daze();
        else
            _enemyAI.Stun(p_stunDuration);

        stats.health -= p_pDamage;

        Knockback(p_pAttacker, 150);
        if (stats.health <= 0)
        {
            _enemyAI.animator.SetTrigger("Die");
            Destroy(this);
            Destroy(_enemyAI);
            GetComponent<Rigidbody2D>().simulated = false;
            GetComponent<Collider2D>().enabled    = false;
        }
    }

    private void Knockback(Player p_pAttacker, float p_pKnockbackForce)
    {
        Vector2 _knockbackDirection = (transform.position - p_pAttacker.PlayerPosition).normalized;

        // GetComponent<Rigidbody2D>().velocity = _knockbackDirection * p_pKnockbackForce;
        GetComponent<Rigidbody2D>().AddForce(_knockbackDirection * p_pKnockbackForce, ForceMode2D.Impulse);
    }
}