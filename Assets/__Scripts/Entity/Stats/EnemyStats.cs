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

    public EnemyStats(EnemyData p_data)
    {
        enemyName      = p_data.enemyName;
        Health         = p_data.stats.Health;
        MaxHealth      = p_data.stats.MaxHealth;
        AttackDamage   = p_data.stats.AttackDamage;
        Speed          = p_data.stats.Speed;
        Level          = p_data.stats.Level;
        Defense        = p_data.stats.Defense;
        AttackSpeed    = p_data.stats.AttackSpeed;
        canDazed       = p_data.stats.canDazed;
        canStun        = p_data.stats.canStun;
        canKnockback   = p_data.stats.canKnockback;
        chaseDistance  = p_data.stats.chaseDistance;
        eliteType      = p_data.stats.eliteType;
        attackType     = p_data.stats.attackType;
        attackRange    = p_data.stats.attackRange;
        stopWhenAttack = p_data.stats.stopWhenAttack;
    }

    public EnemyStats(EnemyStats p_other)
    {
        enemyName      = p_other.enemyName;
        Health         = p_other.Health;
        MaxHealth      = p_other.MaxHealth;
        AttackDamage   = p_other.AttackDamage;
        Speed          = p_other.Speed;
        Level          = p_other.Level;
        Defense        = p_other.Defense;
        AttackSpeed    = p_other.AttackSpeed;
        canDazed       = p_other.canDazed;
        canStun        = p_other.canStun;
        canKnockback   = p_other.canKnockback;
        chaseDistance  = p_other.chaseDistance;
        eliteType      = p_other.eliteType;
        attackType     = p_other.attackType;
        attackRange    = p_other.attackRange;
        stopWhenAttack = p_other.stopWhenAttack;
    }
}