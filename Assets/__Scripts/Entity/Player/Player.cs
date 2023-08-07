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

    private void LoadSettings(PlayerData p_playerData)
    {
        _stats = new PlayerStats(p_playerData.stats);
        UIElements.Instance.SetHealth(_stats.health, _stats.maxHealth);

        _playerController      = gameObject.AddComponent<PlayerController>();
        _animator              = GetComponent<Animator>();
        Controller.player      = this;
        Controller.playerStats = _stats;

        _animator.runtimeAnimatorController = Animation.GetAnimatorController(p_playerData.characterName);

        skills.Add(SkillTypes.Passive,   gameObject.AddComponent(Type.GetType(p_playerData.skillPassive)) as Skill);
        skills.Add(SkillTypes.Primary,   gameObject.AddComponent(Type.GetType(p_playerData.skillPrimary)) as Skill);
        skills.Add(SkillTypes.Secondary, gameObject.AddComponent(Type.GetType(p_playerData.skillSecondary)) as Skill);
        skills.Add(SkillTypes.Utility,   gameObject.AddComponent(Type.GetType(p_playerData.skillUtility)) as Skill);
        skills.Add(SkillTypes.Ultimate,  gameObject.AddComponent(Type.GetType(p_playerData.skillUltimate)) as Skill);

        _statusFeedback = GetComponentInChildren<MMF_Player>();
        _floatingText   = _statusFeedback.GetFeedbackOfType<MMF_FloatingText>();
    }

    void Start()
    {
        LoadSettings(GameManager.CharactersData["Astro"]);
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
        bool _stillAlive = HealthChange(-p_pDamage);

        if (_stillAlive) return; //아래는 죽었을때 이벤트 처리
        _animator.SetTrigger("Death");
        Controller.SetControllable(false);
        Die();
    }

    private bool HealthChange(int p_pDamage)
    {
        Stats.health += p_pDamage;
        if (clientID == GameManager.Instance.playerID) //해당 캐릭터가 자신의 캐릭터일때만 UI 업데이트
            UIElements.Instance.SetHealth(Stats.health, Stats.maxHealth);
        
        return Stats.health > 0; //체력이 0이하면 false 반환
    }

    private void Die()
    {
        
    }
}