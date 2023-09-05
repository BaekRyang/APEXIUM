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
    [SerializeField] private Skill      _skill;
    [DoNotSerialize] private Image      _mainImage;
    [DoNotSerialize] private GameObject _cooldownObject;
    [DoNotSerialize] private TMP_Text   _cooldownText;
    [DoNotSerialize] private Image      _cooldownImage;

    private void Awake()
    {
        _mainImage      = transform.GetComponent<Image>();
        _cooldownObject = transform.Find("Disabled").gameObject;
        _cooldownText   = _cooldownObject.transform.GetChild(0).GetComponent<TMP_Text>();
        _cooldownImage  = _cooldownObject.GetComponent<Image>();
    }

    private void Start() => 
        skillBlocks[skillType] = this;

    private void Update()
    {
        if (_skill == null)
        {
            Initialize();
            return;
        }

        if (_skill.Cooldown <= 0) return; //쿨타임 없으면 업데이트 안함
        SetCoolDown(_skill.Cooldown, _skill.RemainingCooldown);
    }

    private void Initialize()
    {
        Player _localPlayer = GameManager.Instance.GetLocalPlayer();
        if (_localPlayer == null) return;
        if (_localPlayer.skills.TryGetValue(skillType, out Skill _loadedPlayerSkill))
            _skill = _loadedPlayerSkill;
    }

    public void SetMainImage(Image _image)
    {
        _mainImage = _image;
    }

    public void SetCoolDown(float _cooldown, float _remainCooldown)
    {
        if (_remainCooldown > 0)
        {
            _cooldownObject.SetActive(true);
            _cooldownText.text = _remainCooldown >= 1 ?
                $"{_remainCooldown:0.0}" //1초 이상이면 소수점 첫째자리까지
                :
                $"{_remainCooldown:0.00}"; //1초 미만이면 소수점 둘째자리까지
            _cooldownImage            = _cooldownObject.GetComponent<Image>();
            _cooldownImage.fillAmount = _remainCooldown / _cooldown;
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