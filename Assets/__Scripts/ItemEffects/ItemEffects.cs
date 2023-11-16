using System.Collections.Generic;
using UnityEngine;

public interface ItemEffects { }

public class FirstAidKit : ItemEffects
{
    private const float COOLDOWN            = 60f;
    private const float STACK_COOLDOWN      = 30f;
    private const float STACK_EFFECT_HEALTH = 0.01f;
    private const float STACK_HEAL_AMOUNT   = 0.05f;

    private static float NextEffectTime = 0f;

    public static bool Effect(Player _owner, Dictionary<int, int> _items)
    {
        if (Time.time < NextEffectTime)
        {
            Debug.Log($"Remain Time : {NextEffectTime - Time.time}");
            return false;
        }

        PlayerStats _ownerStats = _owner.Stats;
        int         _itemAmount = _items[0];

        NextEffectTime = Time.time + COOLDOWN + STACK_COOLDOWN * (_itemAmount - 1);

        float _remainHealthPercent = (float)_ownerStats.Health / _ownerStats.MaxHealth;
        Debug.Log($"<color=blue>Remain Health Percent : {_remainHealthPercent}</color> - {_itemAmount}");
        if (_remainHealthPercent < 0.1f + _itemAmount * STACK_EFFECT_HEALTH)
            _ownerStats.Health += (int)(_ownerStats.MaxHealth * (_itemAmount * STACK_HEAL_AMOUNT));
        return false;
    }
}

public class StunGranade : ItemEffects
{
    private const float PERCENTAGE       = 0.05f;
    private const float STACK_PERCENTAGE = 0.03f;
    private const float STUN_DURATION    = 2f;
    private const float EFFECT_AREA      = 5f;

    private static Collider2D[] _results;

    public static void Effect(EnemyBase _targetEnemy, Dictionary<int, int> _items)
    {
        int _itemCount = _items[4];
        if (Random.Range(0f, 1f) > PERCENTAGE + STACK_PERCENTAGE * (_itemCount - 1))
            return;

        Physics2D.OverlapCircleNonAlloc(_targetEnemy.transform.position, EFFECT_AREA, _results, LayerMask.GetMask("Enemy"));
        foreach (Collider2D _enemies in _results)
            if (_enemies.TryGetComponent(out EnemyBase _enemy))
                _enemy.Stun(STUN_DURATION);
    }
}