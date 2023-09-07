using System;
using UnityEngine;

[Serializable]
public enum EnemyAttackType
{
    Normal,
    Aura
}

public enum EliteType
{
    None,
    Flame,     //Blazing Step
    Frost,     //Slow
    Lightning, //Static Electricity
    Poison,    //Poison
    Shadow,    //High Damage
    Earth,     //High Health
    Wind       //High Speed
}

[Serializable]
public class EnemyStats : Stats
{
    [SerializeField] public string          enemyName;
    [SerializeField] public bool            canDazed;
    [SerializeField] public bool            canStun;
    [SerializeField] public bool            canKnockback;
    [SerializeField] public float           chaseDistance;
    [SerializeField] public EliteType       eliteType;
    [SerializeField] public EnemyAttackType attackType;
    [SerializeField] public float           attackRange;
    [SerializeField] public bool            stopWhenAttack;

    public EnemyStats(EnemyData _other)
    {
        enemyName      = _other.enemyName;
        Health         = _other.stats.Health;
        maxHealth      = _other.stats.MaxHealth;
        attackDamage   = _other.stats.AttackDamage;
        speed          = _other.stats.Speed;
        level          = _other.stats.Level;
        defense        = _other.stats.Defense;
        attackSpeed    = _other.stats.AttackSpeed;
        canDazed       = _other.stats.canDazed;
        canStun        = _other.stats.canStun;
        canKnockback   = _other.stats.canKnockback;
        chaseDistance  = _other.stats.chaseDistance;
        eliteType      = _other.stats.eliteType;
        attackType     = _other.stats.attackType;
        attackRange    = _other.stats.attackRange;
        stopWhenAttack = _other.stats.stopWhenAttack;
    }
}