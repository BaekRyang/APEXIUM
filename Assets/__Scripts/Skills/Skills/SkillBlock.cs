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
    private GameObject _cooldownImage;
    private TMP_Text   _cooldownText;

    private void Awake()
    {
        _blockSize     = transform.GetComponent<RectTransform>().rect.width;
        _mainImage     = transform.GetComponent<Image>();
        _cooldownImage = transform.Find("Disabled").gameObject;
        _cooldownText  = _cooldownImage.transform.GetChild(0).GetComponent<TMP_Text>();
    }

    public void SetMainImage(Image p_image)
    {
        _mainImage = p_image;
    }

    public void SetCoolDown(float p_cooldown)
    {
        if (p_cooldown > 0)
        {
            _cooldownImage.SetActive(true);
            _cooldownText.text = p_cooldown >= 1 ? $"{p_cooldown:0.0}" : $"{p_cooldown:0.00}"; //1초 이상이면 정수, 1초 미만이면 소수점 첫째자리까지
        }
        else
            _cooldownImage.SetActive(false);
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