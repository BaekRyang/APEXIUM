using System;
using System.Collections.Generic;
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

    private Dictionary<ChangeableStatsTypes, float> _basicStat       = new(); //초기 스텟값
    private Dictionary<ChangeableStatsTypes, float> _statAddSum      = new(); //스텟에 더해지는 값들의 합
    private Dictionary<ChangeableStatsTypes, float> _statMultipliers = new(); //스텟에 곱해지는 값들의 합

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

        //_basicStat을 열거형을 순회하여 채운다.
        foreach (ChangeableStatsTypes _type in Tools.GetEnumValues<ChangeableStatsTypes>())
        {
            _basicStat.Add(_type, (float)GetValue(_type));
            _statAddSum.Add(_type, 0);
            _statMultipliers.Add(_type, 1);
        }
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
        set
        {
            energyCrystal = value;
            if (isLocalPlayer) UpdatableUIElements.UpdateValue("EnergyCrystal", energyCrystal);
        }
    }

    public int AdvancedResource
    {
        get => advancedResource;
        set => advancedResource = value;
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

    private double GetValue(ChangeableStatsTypes type)
    {
        //type에 따라 각 값을 리턴한다
        return type switch
        {
            ChangeableStatsTypes.Health         => Health,
            ChangeableStatsTypes.MaxHealth      => MaxHealth,
            ChangeableStatsTypes.AttackDamage   => AttackDamage,
            ChangeableStatsTypes.Speed          => Speed,
            ChangeableStatsTypes.Defense        => Defense,
            ChangeableStatsTypes.AttackSpeed    => AttackSpeed,
            ChangeableStatsTypes.MaxJumpCount   => MaxJumpCount,
            ChangeableStatsTypes.JumpHeight     => JumpHeight,
            ChangeableStatsTypes.Resource       => Resource,
            ChangeableStatsTypes.MaxResource    => MaxResource,
            ChangeableStatsTypes.EnergyCrystal  => EnergyCrystal,
            ChangeableStatsTypes.CriticalChance => CriticalChance,
            ChangeableStatsTypes.CriticalDamage => CriticalDamage,
            _                                   => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    /// <summary>
    /// type에 따라 각 값을 설정한다 형변환은 여기서 이루어짐
    /// </summary>
    private void SetValue(ChangeableStatsTypes type, double v)
    {
        //type에 따라 각 값을 설정한다 형변환도 적절하게한다
        switch (type)
        {
            case ChangeableStatsTypes.Health:
                Health += (int)v; //체력값의 변화는 증감으로만 사용
                break;
            case ChangeableStatsTypes.MaxHealth:
                MaxHealth = (int)v;
                break;
            case ChangeableStatsTypes.AttackDamage:
                AttackDamage = (int)v;
                break;
            case ChangeableStatsTypes.Speed:
                Speed = (float)v;
                break;
            case ChangeableStatsTypes.Defense:
                Defense = (int)v;
                break;
            case ChangeableStatsTypes.AttackSpeed:
                AttackSpeed = (float)v;
                break;
            case ChangeableStatsTypes.MaxJumpCount:
                MaxJumpCount = (int)v;
                break;
            case ChangeableStatsTypes.JumpHeight:
                JumpHeight = (float)v;
                break;
            case ChangeableStatsTypes.Resource:
                Resource = (int)v;
                break;
            case ChangeableStatsTypes.MaxResource:
                MaxResource = (int)v;
                break;
            case ChangeableStatsTypes.EnergyCrystal:
                EnergyCrystal = (int)v;
                break;
            case ChangeableStatsTypes.CriticalChance:
                CriticalChance = (float)v;
                break;
            case ChangeableStatsTypes.CriticalDamage:
                CriticalDamage = (float)v;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void ApplyStats(StatModifier _mod, bool _isRevert = false)
    {
        double _targetValue = _basicStat[_mod.statType];
        float  _value       = _mod.value;

        //체력 변화는 값을 저장해서 사용하지 않음
        if (_mod.statType == ChangeableStatsTypes.Health)
        {
            SetValue(_mod.statType, _isRevert ? -_value : _value);
            return;
        }

        switch (_mod.calculationType)
        {
            case CalculationType.Add:
                _statAddSum[_mod.statType] += _isRevert ? -_value : _value;
                break;
            case CalculationType.Multiply:
                _statMultipliers[_mod.statType] += _isRevert ? -(_value - 1) : (_value - 1);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _targetValue += _statAddSum[_mod.statType];
        _targetValue *= _statMultipliers[_mod.statType];
        SetValue(_mod.statType, _targetValue);
    }
}