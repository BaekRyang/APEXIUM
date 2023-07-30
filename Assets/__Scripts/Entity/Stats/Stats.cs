using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class Stats<T> where T : Stats<T>
{
    //제네릭 자기참조 형식으로 정의되었기 때문에, "T"는 Stats<T>를 상속받는 클래스여야 한다.
    //Stats는 자신을 상속받아야 하기 때문에 인스턴스화 할 수 없다.
    public int   health;
    public int   maxHealth;
    public int   attackDamage;
    public float speed;
    public int   level;
    public int   defense;
    public float attackSpeed;

    public T SetHealth(int p_health)
    {
        health = maxHealth = p_health;
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