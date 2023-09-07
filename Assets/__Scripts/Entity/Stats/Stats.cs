using System;
using UnityEngine;

[Serializable]
public class Stats
{
    [SerializeField] public bool isLocalPlayer;

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
        set
        {
            health = value;
            if (isLocalPlayer) UpdatableUIElements.UpdateValue("HP", health, maxHealth);
        }
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
        set
        {
            level = value;
            Debug.Log("LVU");
            if (isLocalPlayer) UpdatableUIElements.UpdateValue("Level", level);
        }
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