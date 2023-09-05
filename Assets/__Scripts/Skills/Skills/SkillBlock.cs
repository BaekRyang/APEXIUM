using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillBlock : MonoBehaviour
{
    public static Dictionary<SkillTypes, SkillBlock> skillBlocks = new();

    [SerializeField] private SkillTypes skillType;
    [SerializeField] private Skill      skill;
    [DoNotSerialize] private                  Image      _mainImage;
    [DoNotSerialize] private                  GameObject _cooldownObject;
    [DoNotSerialize] private                  TMP_Text   _cooldownText;
    [DoNotSerialize] private                  Image      _cooldownImage;

    private void Awake()
    {
        _mainImage      = transform.GetComponent<Image>();
        _cooldownObject = transform.Find("Disabled").gameObject;
        _cooldownText   = _cooldownObject.transform.GetChild(0).GetComponent<TMP_Text>();
        _cooldownImage  = _cooldownObject.GetComponent<Image>();
    }

    private void Start()
    {
        skillBlocks[skillType] = this;
    }

    private void Update()
    {
        if (skill == null)
        {
            Initialize();
            return;
        }

        if (skill.Cooldown <= 0) return; //쿨타임 없으면 업데이트 안함
        SetCoolDown(skill.Cooldown, skill.RemainingCooldown);
    }

    private void Initialize()
    {
        Player _localPlayer = GameManager.Instance.GetLocalPlayer();
        if (_localPlayer == null) return;
        if (_localPlayer.skills.TryGetValue(skillType, out Skill _loadedPlayerSkill))
            skill = _loadedPlayerSkill;

    }

    public void SetMainImage(Image p_image)
    {
        _mainImage = p_image;
    }

    public void SetCoolDown(float p_cooldown, float p_remainCooldown)
    {
        if (p_remainCooldown > 0)
        {
            _cooldownObject.SetActive(true);
            _cooldownText.text = p_remainCooldown >= 1 ?
                $"{p_remainCooldown:0.0}" //1초 이상이면 소수점 첫째자리까지
                :
                $"{p_remainCooldown:0.00}"; //1초 미만이면 소수점 둘째자리까지
            _cooldownImage            = _cooldownObject.GetComponent<Image>();
            _cooldownImage.fillAmount = p_remainCooldown / p_cooldown;
        }
        else
            _cooldownObject.SetActive(false);
    }
}

[Serializable]
public enum SkillTypes
{
    Primary,
    Secondary,
    Utility,
    Ultimate,
    Passive,
    Item
}