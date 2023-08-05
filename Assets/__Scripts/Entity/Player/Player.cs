using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    public int clientID;

    private PlayerController _playerController;
    private PlayerStats      _stats;
    private bool             _isImmune;
    public  Vector3          PlayerPosition => transform.position;

    public readonly Dictionary<SkillTypes, Skill> skills = new Dictionary<SkillTypes, Skill>();

    private MMF_Player       _statusFeedback;
    private MMF_FloatingText _floatingText;

    public Animator         _animator;
    public PlayerController Controller => _playerController;

    public PlayerStats Stats => _stats;

    private void LoadSettings()
    {
        _stats = new PlayerStats()
                .SetOwnerID(GameManager.Instance.playerID)
                .SetHealth(100)
                .SetAttackDamage(10)
                .SetSpeed(4f)
                .SetDefense(0)
                .SetAttackSpeed(1f)
                .SetMaxJumpCount(1)
                .SetJumpHeight(10)
                .SetResource(7);
        UIElements.Instance.SetHealth(_stats.health, _stats.maxHealth);

        _playerController      = gameObject.AddComponent<PlayerController>();
        _animator              = GetComponent<Animator>();
        Controller.player      = this;
        Controller.playerStats = _stats;

        _animator.runtimeAnimatorController = Animation.GetAnimatorController("Astro");
        skills.Add(SkillTypes.Passive,   gameObject.AddComponent<Revolver>());
        skills.Add(SkillTypes.Primary,   gameObject.AddComponent<RevolverShot>());
        skills.Add(SkillTypes.Secondary, gameObject.AddComponent<PierceShot>());
        skills.Add(SkillTypes.Utility,   gameObject.AddComponent<Roll>());
        skills.Add(SkillTypes.Ultimate,  gameObject.AddComponent<Spree>());

        var a = typeof(Spree);
        gameObject.AddComponent(a);
        
        _statusFeedback = GetComponentInChildren<MMF_Player>();
        _floatingText   = _statusFeedback.GetFeedbackOfType<MMF_FloatingText>();
    }

    void Start()
    {
        LoadSettings();
    }

    public void PlayStatusFeedback(string p_pText)
    {
        if (_statusFeedback.IsUnityNull())
        {
            Debug.Log("NULL");
            return;
        }

        _floatingText.Value = p_pText;
        _statusFeedback.PlayFeedbacks();
    }

    public void Attacked(int p_pDamage, float p_stunDuration, EnemyBase p_pAttacker)
    {
        if (_isImmune) return;

        PlayStatusFeedback(p_pDamage.ToString());
        TakeDamage(p_pDamage);
    }

    private void TakeDamage(int p_pDamage)
    {
        Stats.health -= p_pDamage;
        UIElements.Instance.SetHealth(Stats.health, Stats.maxHealth);
    }

    public void ChangeStats(StatType p_statType, float p_value)
    {
        switch (p_statType)
        {
            case StatType.Health:
                Stats.health += (int)p_value;
                break;
            case StatType.MaxHealth:
                Stats.maxHealth += (int)p_value;
                break;
            case StatType.AttackDamage:
                Stats.attackDamage += (int)p_value;
                break;
            case StatType.Speed:
                Stats.speed += p_value;
                break;
            case StatType.Level:
                
                break;
            case StatType.Defense:
                break;
            case StatType.AttackSpeed:
                break;
            case StatType.Exp:
                break;
            case StatType.MaxExp:
                break;
            case StatType.MaxJumpCount:
                break;
            case StatType.JumpHeight:
                break;
            case StatType.Resource:
                break;
            case StatType.MaxResource:
                break;
            case StatType.CommonResource:
                break;
            case StatType.AdvancedResource:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(p_statType), p_statType, null);
        }
    }

}