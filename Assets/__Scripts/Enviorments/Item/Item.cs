using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic
}

[Serializable]
public enum ChangeableStatsTypes
{
    MaxHealth,
    AttackDamage,
    Speed,
    Defense,
    AttackSpeed,
    MaxJumpCount,
    JumpHeight,
    Resource,
    MaxResource,
    EnergyCrystal,
    CriticalChance,
    CriticalDamage
}

[Serializable]
public class Item
{
    public int        id;
    public string     name;
    public ItemRarity rarity;
    public Sprite     sprite;

    [Space(20)]
    public List<StatModifier> statValues;
    public List<Effect> effect;
}

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemList : ScriptableObject
{
    public List<Item> items;
}

public enum TriggerCondition
{
    OnAttack,
    OnHit,
}

public enum FxType
{
    AddMana
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

[Serializable]
public class Fx
{
    public FxType      fxType;
    public List<float> values;
}

[Serializable]
public class Effect
{
    public TriggerCondition triggerCondition;
    public List<Fx>         fxes;
}