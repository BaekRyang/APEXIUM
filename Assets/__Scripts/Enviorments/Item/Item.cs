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
    Health,
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
    [SerializeField] public int        id;
    [SerializeField] public string     name;
    [SerializeField] public ItemRarity rarity = ItemRarity.Common;
    [SerializeField] public Sprite     sprite;

    [Space(20)]
    [SerializeField] public List<StatModifier> statValues;

    [SerializeField] public bool hasSpecialEffect;
}