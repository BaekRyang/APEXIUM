using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElements : MonoBehaviour
{
    public static UIElements Instance;

    public Dictionary<SkillTypes, SkillBlock> skillBlocks = new Dictionary<SkillTypes, SkillBlock>();

    public Slider     healthBar;
    public TMP_Text   healthIndex;
    public Slider     expBar;
    public TMP_Text   levelIndex;
    public CellSlider resourceBar;

    private void Awake()
    {
        Instance ??= this;

        Transform _playerUI = transform.Find("PlayerUI");
        foreach (Transform _block in _playerUI.Find("Blocks"))
        {
            SkillBlock _skillBlock = _block.GetComponent<SkillBlock>();
            skillBlocks.Add(_skillBlock.skillType, _skillBlock);
        }

        healthBar   = _playerUI.Find("Gauges").Find("HP").GetComponent<Slider>();
        healthIndex = healthBar.transform.Find("Value").GetComponent<TMP_Text>();

        expBar     = _playerUI.Find("EXP").GetComponent<Slider>();
        levelIndex = expBar.transform.Find("Value").GetComponent<TMP_Text>();

        resourceBar = _playerUI.Find("Gauges").Find("Resources").GetComponent<CellSlider>();
    }

    private int _cachedMaxHealth = -1;

    public void SetHealth(int p_currentHealth = -1, int p_maxHealth = -1)
    {
        if (_cachedMaxHealth == -1) _cachedMaxHealth = GameManager.Instance.GetLocalPlayer().Stats.maxHealth;
        
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

    public void SetCoolDown(SkillTypes p_skillType, float p_cooldown) => skillBlocks[p_skillType].SetCoolDown(p_cooldown);

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