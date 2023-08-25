using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillBlock : MonoBehaviour
{
    public  SkillTypes skillType;
    private float      _blockSize;
    private Image      _mainImage;
    private GameObject _cooldownObject;
    private TMP_Text   _cooldownText;
    private Image      _cooldownImage;

    private void Awake()
    {
        _blockSize     = transform.GetComponent<RectTransform>().rect.width;
        _mainImage     = transform.GetComponent<Image>();
        _cooldownObject = transform.Find("Disabled").gameObject;
        _cooldownText  = _cooldownObject.transform.GetChild(0).GetComponent<TMP_Text>();
        _cooldownImage = _cooldownObject.GetComponent<Image>();
    }

    private void Start()
    {
        UIElements.Instance.AddSkillBlock(skillType, this);
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
            _cooldownText.text = p_remainCooldown >= 1
                ? $"{p_remainCooldown:0.0}"   //1초 이상이면 소수점 첫째자리까지
                : $"{p_remainCooldown:0.00}"; //1초 미만이면 소수점 둘째자리까지
            _cooldownImage                                   = _cooldownObject.GetComponent<Image>();
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