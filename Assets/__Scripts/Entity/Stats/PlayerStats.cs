using System;
using UnityEngine;
using UnityEngine.Serialization;

public enum StatType
{
    Health,
    MaxHealth,
    AttackDamage,
    Speed,
    Level,
    Defense,
    AttackSpeed,
    Exp,
    MaxExp,
    MaxJumpCount,
    JumpHeight,
    Resource,
    MaxResource,
    CommonResource,
    AdvancedResource
}

[Serializable]
public class PlayerStats : Stats<PlayerStats>
{
    [SerializeField] private int   ownerID;
    [SerializeField] private int   exp;
    [SerializeField] private int   maxExp = 100;
    [SerializeField] private int   maxJumpCount;
    [SerializeField] private float jumpHeight;
    [SerializeField] private int   resource;
    [SerializeField] private int   maxResource;
    [SerializeField] private int   commonResource;
    [SerializeField] private int   advancedResource;

    public PlayerStats(PlayerStats p_other)
    {
        ownerID         = p_other.ownerID;
        health          = p_other.health;
        maxHealth       = p_other.maxHealth;
        attackDamage    = p_other.attackDamage;
        speed           = p_other.speed;
        level           = p_other.level;
        defense         = p_other.defense;
        attackSpeed     = p_other.attackSpeed;
        exp             = p_other.exp;
        maxExp          = p_other.maxExp;
        maxJumpCount    = p_other.maxJumpCount;
        jumpHeight      = p_other.jumpHeight;
        resource        = p_other.resource;
        maxResource     = p_other.maxResource;
        commonResource  = p_other.commonResource;
        advancedResource = p_other.advancedResource;
    }

    public int OwnerID => ownerID;

    public int Resource
    {
        get => resource;
        set => UIElements.Instance.resourceBar.value = resource = value;
    }

    public int MaxResource
    {
        get => maxResource;
        set
        {
            UIElements.Instance.resourceBar.maxValue = maxResource = value;
            UIElements.Instance.resourceBar.ApplySetting();
        }
    }

    public float JumpHeight
    {
        get => jumpHeight;
        set => jumpHeight = value;
    }

    public int MaxJumpCount
    {
        get => maxJumpCount;
        set => maxJumpCount = value;
    }

    public int CommonResource
    {
        get => commonResource;
        set
        {
            commonResource                        = value;
            Resources.Instance.ResourceValue.text = value.ToString();
        }
    }

    public int AdvancedResource
    {
        get => advancedResource;
        set
        {
            advancedResource                              = value;
            Resources.Instance.AdvancedResourceValue.text = value.ToString();
        }
    }
}