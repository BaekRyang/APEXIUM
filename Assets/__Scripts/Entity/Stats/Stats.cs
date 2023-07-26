using System;
using UnityEngine.Serialization;

[Serializable]
public class Stats
{
    private int   _health;
    public  int   attackDamage;
    public  float speed;
    public  int   level;
    public  int   defense;
    public  float attackSpeed;

    protected Stats()
    {
        Health       = 100;
        attackDamage = 30;
        speed        = 2f;
        level        = 1;
        defense      = 0;
        attackSpeed  = 1f;
    }

    public static Stats CreateInstance()
    {
        return new Stats();
    }

    public int Health
    {
        get => _health;
        set => _health = value;
    }


}