using System.Collections.Generic;
using UnityEngine;

public static class FirstAidKit
{
    private const  float COOLDOWN       = 60f;
    private const  float STACK_COOLDOWN  = 30f;
    private const  float STACK_EFFECT_HEALTH = 0.01f;
    private const  float STACK_HEAL_AMOUNT   = 0.05f;
    
    private static float NextEffectTime    = 0f;

    public static bool Effect(Player _owner, Dictionary<int, int> _items)
    {
        
        if (Time.time < NextEffectTime)
        {
            Debug.Log($"Remain Time : {NextEffectTime - Time.time}");
            return false;
        }

        PlayerStats _ownerStats = _owner.Stats;
        int        _itemAmount = _items[0];
        
        NextEffectTime = Time.time + COOLDOWN + STACK_COOLDOWN * (_itemAmount - 1);

        float _remainHealthPercent = (float)_ownerStats.Health / _ownerStats.MaxHealth;
        Debug.Log($"<color=blue>Remain Health Percent : {_remainHealthPercent}</color> - {_itemAmount}");
        if (_remainHealthPercent < 0.1f + _itemAmount * STACK_EFFECT_HEALTH)
            _ownerStats.Health += (int)(_ownerStats.MaxHealth * (_itemAmount * STACK_HEAL_AMOUNT));
        return false;
    }
}