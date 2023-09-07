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
    [SerializeField] protected int   energyCrystal;
    [SerializeField] protected int   advancedResource;
    [SerializeField] protected float criticalChance;
    [SerializeField] protected float criticalDamage;

    public PlayerStats(PlayerStats _other)
    {
        health           = _other.Health;
        maxHealth        = _other.MaxHealth;
        attackDamage     = _other.AttackDamage;
        speed            = _other.Speed;
        level            = _other.Level;
        defense          = _other.Defense;
        attackSpeed      = _other.AttackSpeed;
        exp              = _other.exp;
        maxExp           = _other.maxExp;
        maxJumpCount     = _other.maxJumpCount;
        jumpHeight       = _other.jumpHeight;
        resource         = _other.resource;
        maxResource      = _other.maxResource;
        energyCrystal    = _other.energyCrystal;
        advancedResource = _other.advancedResource;
        criticalChance   = _other.CriticalChance;
        criticalDamage   = _other.CriticalDamage;
    }

    public int Resource
    {
        get => resource;
        set => resource = value;
    }

    public int MaxResource
    {
        get => maxResource;
        set => maxResource = value;
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

    public int EnergyCrystal
    {
        get => energyCrystal;
        set => energyCrystal = value;
    }

    public int AdvancedResource
    {
        get => advancedResource;
        set => advancedResource = value;
    }

    public float CriticalChance
    {
        get => criticalChance > 1 ? 1 : criticalChance;
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
            else if (isLocalPlayer) UpdatableUIElements.UpdateValue("EXP", exp, maxExp);
        }
    }

    private async void LevelUp()
    {
        do
        {
            exp    -= maxExp;
            maxExp += Convert.ToInt32(maxExp * 0.2f);
            Level++;
            await UniTask.Delay(10); //TODO: 무한 루프 방지용(없애야함)
        } while (exp >= maxExp);
    }
}