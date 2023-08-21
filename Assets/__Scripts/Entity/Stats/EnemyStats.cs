using System;
using UnityEngine;

[Serializable]
public enum EnemyAttackType
{
    Normal,
    Aura
}

[Serializable]
public class EnemyStats : Stats<EnemyStats>
{
    [SerializeField] public string          enemyName;
    [SerializeField] public bool            canDazed;
    [SerializeField] public bool            canStun;
    [SerializeField] public float           chaseDistance;
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
        chaseDistance  = p_data.stats.chaseDistance;
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
        chaseDistance  = p_other.chaseDistance;
        attackType     = p_other.attackType;
        attackRange    = p_other.attackRange;
        stopWhenAttack = p_other.stopWhenAttack;
    }
}