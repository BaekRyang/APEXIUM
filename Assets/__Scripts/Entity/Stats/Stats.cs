using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class Stats<T> where T : Stats<T>
{
    public int   health;
    public int   maxHealth;
    public int   attackDamage;
    public float speed;
    public int   level;
    public int   defense;
    public float attackSpeed;

    public int Health
    {
        get => health;
        set
        {
            health = value;
            if (this is PlayerStats)
            {
                Debug.Log("Change Player Stats");
                UIElements.Instance.SetHealth(health, maxHealth);
            }
        }
    }
    
    public T SetHealth(int p_health)
    {
        Health = maxHealth = p_health;
        return (T)this;
    }
    
    public T SetAttackDamage(int p_attackDamage)
    {
        attackDamage = p_attackDamage;
        return (T)this;
    }
    
    public T SetSpeed(float p_speed)
    {
        speed = p_speed;
        return (T)this;
    }
    
    public T SetDefense(int p_defense)
    {
        defense = p_defense;
        return (T)this;
    }
    
    public T SetAttackSpeed(float p_attackSpeed)
    {
        attackSpeed = p_attackSpeed;
        return (T)this;
    }
    
    
}