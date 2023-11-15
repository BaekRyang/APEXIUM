using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
    public List<Item> items;
}

public enum TriggerCondition
{
    OnAttackExecute,
    OnAttackHit,
    OnAttacked,
    OnDead
}


[Serializable]
public enum CalculationType
{
    Add,
    Multiply
}

[Serializable]
public class StatModifier
{
    public ChangeableStatsTypes statType;
    public CalculationType      calculationType;
    public float                value;
}
