using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

        resourceBar = _playerUI.Find("Gauges").Find("Resource").GetComponent<CellSlider>();
    }

    public void SetHealth(float p_currentHealth, float p_maxHealth)
    {
        healthBar.maxValue = p_maxHealth;
        healthBar.value  = p_currentHealth;
        healthIndex.text = $"{p_currentHealth:0}/{p_maxHealth:0}";
    }

    public void SetExp(float p_currentExp, float p_maxExp) => expBar.value = p_currentExp / p_maxExp;

    public void SetLevelIndex(int p_level) => levelIndex.text = $"{p_level}";
    
    public void SetCoolDown(SkillTypes p_skillType, float p_cooldown) => skillBlocks[p_skillType].SetCoolDown(p_cooldown);
}