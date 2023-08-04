using MoreMountains.Feedbacks;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    private EnemyAI          _enemyAI;
    private MMF_Player       _damageFeedback;
    private MMF_FloatingText _floatingText;

    public MonsterStats stats;

    private void Start()
    {
        stats = new MonsterStats()
               .SetHealth(200)
               .SetAttackDamage(10)
               .SetSpeed(4f)
               .SetDefense(1)
               .SetAttackSpeed(.5f);

        _enemyAI = GetComponent<EnemyAI>();
        _enemyAI.Initialize(this);
        _damageFeedback = transform.Find("DamageFeedback").GetComponent<MMF_Player>();
        _floatingText   = _damageFeedback.GetFeedbackOfType<MMF_FloatingText>();

    }

    public void Attacked(int p_pDamage, float p_stunDuration, Player p_pAttacker)
    {
        _floatingText.Value = p_pDamage.ToString();
        _damageFeedback.PlayFeedbacks();

        if (p_stunDuration == 0)
            _enemyAI.Daze();
        else
            _enemyAI.Stun(p_stunDuration);

        stats.health -= p_pDamage;

        Knockback(p_pAttacker, 150);
        if (stats.health <= 0)
        {
            _enemyAI._animator.SetTrigger("Die");
            Destroy(this);
            Destroy(_enemyAI);
            GetComponent<Rigidbody2D>().simulated = false;
            GetComponent<Collider2D>().enabled   = false;
        }
    }

    private void Knockback(Player p_pAttacker, float p_pKnockbackForce)
    {
        Vector2 _knockbackDirection = (transform.position - p_pAttacker.PlayerPosition).normalized;

        // GetComponent<Rigidbody2D>().velocity = _knockbackDirection * p_pKnockbackForce;
        GetComponent<Rigidbody2D>().AddForce(_knockbackDirection * p_pKnockbackForce, ForceMode2D.Impulse);
    }
}