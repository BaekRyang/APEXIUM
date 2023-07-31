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

    private MMF_Player _statusFeedback;
    private MMF_FloatingText _floatingText;
    public PlayerController Controller => _playerController;

    public PlayerStats Stats => _stats;

    private void LoadSettings()
    {
        _stats = new PlayerStats()
                .SetHealth(100)
                .SetAttackDamage(10)
                .SetSpeed(4f)
                .SetDefense(0)
                .SetAttackSpeed(1f)
                .SetMaxJumpCount(1)
                .SetJumpHeight(10)
                .SetResource(7);


        _playerController      = gameObject.AddComponent<PlayerController>();
        Controller.player      = this;
        Controller.playerStats = _stats;

        skills.Add(SkillTypes.Passive,   gameObject.AddComponent<Revolver>());
        skills.Add(SkillTypes.Primary,   gameObject.AddComponent<RevolverShot>());
        skills.Add(SkillTypes.Secondary, gameObject.AddComponent<PierceShot>());
        skills.Add(SkillTypes.Utility,   gameObject.AddComponent<Roll>());
        skills.Add(SkillTypes.Ultimate,  gameObject.AddComponent<Spree>());

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
        
        TakeDamage(p_pDamage);
    }
    
    private void TakeDamage(int p_pDamage)
    {
        Stats.health -= p_pDamage;
    }
}