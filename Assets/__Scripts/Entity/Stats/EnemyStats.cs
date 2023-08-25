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
public class EnemyStats : Stats<EnemyStats>
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
        health         = p_data.stats.health;
        maxHealth      = p_data.stats.maxHealth;
        attackDamage   = p_data.stats.attackDamage;
        speed          = p_data.stats.speed;
        level          = p_data.stats.level;
        defense        = p_data.stats.defense;
        attackSpeed    = p_data.stats.attackSpeed;
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
        health         = p_other.health;
        maxHealth      = p_other.maxHealth;
        attackDamage   = p_other.attackDamage;
        speed          = p_other.speed;
        level          = p_other.level;
        defense        = p_other.defense;
        attackSpeed    = p_other.attackSpeed;
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