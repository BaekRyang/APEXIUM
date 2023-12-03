using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
        Debug.Log($"<color=blue> {0.1f + _itemAmount * STACK_EFFECT_HEALTH}");
        if (_remainHealthPercent < 0.1f + _itemAmount * STACK_EFFECT_HEALTH)
            _ownerStats.Health += (int)(_ownerStats.MaxHealth * (_itemAmount * STACK_HEAL_AMOUNT));
        return false;
    }
}

public class StunGrenade : ItemEffects
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

public class ToePopper : ItemEffects
{
    private static readonly GameObject MineObject = Addressables.LoadAssetAsync<GameObject>("Assets/_Prefabs/Objects/InGameObject/ToePopper.prefab").WaitForCompletion();

    private const float PERCENTAGE       = 0.1f;
    private const float STACK_PERCENTAGE = 0.05f;
    private const float DAMAGE           = 3.0f;
    private const float STACK_DAMAGE     = 0.5f;
    private const float EFFECT_DURATION  = 3.0f;
    private const float EFFECT_STRENGTH  = 0.5f;

    public static bool Effect(Player _owner, Dictionary<int, int> _items)
    {
        int _itemCount = _items[5];
        if (Random.Range(0f, 1f) > PERCENTAGE + STACK_PERCENTAGE * (_itemCount - 1))
            return false;

        GameObject.Instantiate(MineObject, _owner.transform.position, Quaternion.identity);
        return false;
    }
}

public class FrostGrenade
{
    private const float PERCENTAGE       = 0.05f;
    private const float STACK_PERCENTAGE = 0.03f;
    private const float DURATION         = 3.0f;
    private const float STRENTH          = 0.2f;
    private const float STACK_STRENGTH   = 0.02f;
    private const float EFFECT_AREA      = 5f;

    private static Collider2D[] _results;

    public static void Effect(EnemyBase _targetEnemy, Dictionary<int, int> _items)
    {
        int _itemCount = _items[6];
        if (Random.Range(0f, 1f) > PERCENTAGE + STACK_PERCENTAGE * (_itemCount - 1))
            return;

        Physics2D.OverlapCircleNonAlloc(_targetEnemy.transform.position, EFFECT_AREA, _results, LayerMask.GetMask("Enemy"));
        foreach (Collider2D _enemies in _results)
            if (_enemies.TryGetComponent(out EnemyBase _enemy))
                _enemy.Stun(DURATION); //TODO: 슬로우로 바꿔야함
    }
}

public class Scouter
{
    public static int Total_Accumulated_Damage = 0;
    
    private const float REQUIRED_DAMAGE_PERCENTAGE       = 0.8f;
    private const float STACK_REQUIRED_DAMAGE_PERCENTAGE = -0.03f;
    private const float DAMAGE_AMPLIFIER                 = 0.1f;
    private const float STACK_DAMAGE_AMPLIFIER           = 0.05f;

    public static void Effect(EnemyBase _targetEnemy, float _damage, Player _owner, Dictionary<int, int> _items)
    {
        if (!_targetEnemy.itemData.TryGetValue("Scouter", out (bool, float) _totalDamageRatio))
        {
            //아직 없으면 추가하고 값 설정하고 리턴
            _targetEnemy.itemData.Add("Scouter", (false, _damage / _targetEnemy.stats.MaxHealth));
            return;
        }

        int _itemStackAmount = _items[8] - 1;
        
        if (_totalDamageRatio.Item1)
        {
            //이미 낙인 발생시 추가 피해를 주고 리턴
            int _additionalDamage = (int)(_damage * (DAMAGE_AMPLIFIER + STACK_DAMAGE_AMPLIFIER * _itemStackAmount));
            _targetEnemy.Attacked(_additionalDamage, false, 0, _owner);
            Total_Accumulated_Damage += _additionalDamage; //통계 누적
            return;
        }

        if (_totalDamageRatio.Item2 < REQUIRED_DAMAGE_PERCENTAGE + STACK_REQUIRED_DAMAGE_PERCENTAGE * _itemStackAmount)
        {
            //데미지가 충분하지 않으면 값 설정하고 리턴
            _targetEnemy.itemData["Scouter"] = (false, _totalDamageRatio.Item2 + _damage / _targetEnemy.stats.MaxHealth);
            return;
        }

        //낙인이 없는데 조건을 충족시키면 낙인 발생
        _targetEnemy.itemData["Scouter"] = (true, _totalDamageRatio.Item2 + DAMAGE_AMPLIFIER + STACK_DAMAGE_AMPLIFIER * _itemStackAmount);
        
    }
}