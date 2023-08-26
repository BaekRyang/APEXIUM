using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class PlayerStats : Stats
{
    [SerializeField] protected int   exp;
    [SerializeField] protected int   maxExp;
    [SerializeField] protected int   maxJumpCount;
    [SerializeField] protected float jumpHeight;
    [SerializeField] protected int   resource;
    [SerializeField] protected int   maxResource;
    [SerializeField] protected int   commonResource;
    [SerializeField] protected int   advancedResource;
    [SerializeField] protected float criticalChance;
    [SerializeField] protected float criticalDamage;

    public PlayerStats(PlayerStats p_other)
    {
        health           = p_other.Health;
        maxHealth        = p_other.MaxHealth;
        attackDamage     = p_other.AttackDamage;
        speed            = p_other.Speed;
        level            = p_other.Level;
        defense          = p_other.Defense;
        attackSpeed      = p_other.AttackSpeed;
        exp              = p_other.exp;
        maxExp           = p_other.maxExp;
        maxJumpCount     = p_other.maxJumpCount;
        jumpHeight       = p_other.jumpHeight;
        resource         = p_other.resource;
        maxResource      = p_other.maxResource;
        commonResource   = p_other.commonResource;
        advancedResource = p_other.advancedResource;
        criticalChance   = p_other.CriticalChance;
        criticalDamage   = p_other.CriticalDamage;
    }

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

    public float CriticalChance
    {
        get => criticalChance > 1 ?
            1 :
            criticalChance;
        set => criticalChance = value;
    }

    public float CriticalDamage
    {
        get
        {
            float _calculatedMultiplier = criticalDamage;

            //100%를 초과하는 크리티컬 확률의 50%만큼 크리티컬 데미지를 증가시킨다.
            if (criticalChance > 1)
                _calculatedMultiplier += (1 - criticalChance) / 2;

            //하한선 설정
            if (_calculatedMultiplier < 1) _calculatedMultiplier = 1;

            return _calculatedMultiplier;
        }
        set => criticalDamage = value;
    }

    public int Exp
    {
        get => exp;
        set
        {
            exp = value;
            if (exp >= maxExp)
                LevelUp();
        }
    }

    private async void LevelUp()
    {
        Debug.Log("levelup");
        do
        {
            exp    -= maxExp;
            Debug.Log($"max exp: {maxExp} => {Convert.ToInt32(maxExp * 1.2f)}");
            maxExp += Convert.ToInt32(maxExp * 0.2f);
            Level++;
            await UniTask.Delay(10);
        } while (exp >= maxExp);
    }
}