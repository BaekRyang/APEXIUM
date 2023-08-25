using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElements : MonoBehaviour
{
    public static UIElements Instance;

    public Dictionary<SkillTypes, SkillBlock> skillBlocks = new();

    public Slider     healthBar;
    public TMP_Text   healthIndex;
    public Slider     expBar;
    public TMP_Text   levelIndex;
    public CellSlider resourceBar;
    private void Awake()
    {
        Instance ??= this;
    }
    
    public void AddSkillBlock(SkillTypes p_skillType, SkillBlock p_skillBlock)
    {
        skillBlocks[p_skillType] = p_skillBlock;
    }

    private int _cachedMaxHealth = -1;

    public void SetHealth(int p_currentHealth = -1, int p_maxHealth = -1)
    {
        if (_cachedMaxHealth == -1) _cachedMaxHealth = GameManager.Instance.GetLocalPlayer().Stats.MaxHealth;
        
        if (p_maxHealth == -1) //최대 체력이 -1이면 기존 최대 체력으로 설정
            p_maxHealth = _cachedMaxHealth;
        else //아니면 최대 체력 갱신
            healthBar.maxValue = _cachedMaxHealth = p_maxHealth;

        if (p_currentHealth == -1) //현재 체력이 -1이면 기존 현재 체력으로 설정
            p_currentHealth = (int)healthBar.value;
        else //아니면 현재 체력 갱신
            healthBar.value = p_currentHealth;

        //체력바 텍스트 갱신
        healthIndex.text = $"{p_currentHealth:0}/{p_maxHealth:0}";
    }

    public void SetExp(int p_currentExp) => expBar.value = p_currentExp;

    public void SetLevelIndex(int p_level) => levelIndex.text = $"{p_level}";

    public void SetCoolDown(SkillTypes p_skillType, float p_cooldown, float p_remainCooldown) =>
        skillBlocks[p_skillType].SetCoolDown(p_cooldown, p_remainCooldown);

    // public void Notified(NotifyTypes p_type, object p_value)
    // {
    //     switch (p_type)
    //     {
    //         case NotifyTypes.Health:
    //             SetHealth(p_currentHealth: (int)p_value);
    //             break;
    //         case NotifyTypes.MaxHealth:
    //             SetHealth(p_maxHealth: (int)p_value);
    //             break;
    //         case NotifyTypes.Exp:
    //             SetExp((int)p_value);
    //             break;
    //         case NotifyTypes.Level:
    //             SetLevelIndex((int)p_value);
    //             break;
    //
    //     }
    // }
}