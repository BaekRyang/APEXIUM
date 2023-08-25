using System;
using UnityEngine;

[Serializable]
public class Stats
{
    [SerializeField] protected int   health       = 100;
    [SerializeField] protected int   maxHealth    = 100;
    [SerializeField] protected int   attackDamage = 10;
    [SerializeField] protected float speed        = 5;
    [SerializeField] protected int   level        = 1;
    [SerializeField] protected int   defense      = 1;
    [SerializeField] protected float attackSpeed  = 1;
    
    public int Health
    {
        get => health;

        set => health = (health + value > MaxHealth) ?
            MaxHealth :
            value; //초과분 생기지 않도록 제한
    }

    public int MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value;
    }

    public int AttackDamage
    {
        get => attackDamage;
        set => attackDamage = value;
    }

    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    public int Level
    {
        get => level;
        set => level = value;
    }

    public int Defense
    {
        get => defense;
        set => defense = value;
    }

    public float AttackSpeed
    {
        get => attackSpeed;
        set => attackSpeed = value;
    }
}