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

    public PlayerStats SetOwnerID(int p_ownerID)
    {
        ownerID = p_ownerID;
        return this;
    }

    public PlayerStats SetMaxJumpCount(int p_maxJumpCount)
    {
        MaxJumpCount = p_maxJumpCount;
        return this;
    }

    public PlayerStats SetJumpHeight(float p_jumpHeight)
    {
        JumpHeight = p_jumpHeight;
        return this;
    }

    public PlayerStats SetResource(int p_resource)
    {
        UIElements.Instance.resourceBar.maxValue = UIElements.Instance.resourceBar.value = p_resource;
        UIElements.Instance.resourceBar.ApplySetting();
        Resource = MaxResource = p_resource;
        return this;
    }
}