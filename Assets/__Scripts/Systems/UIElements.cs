using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElements : MonoBehaviour
{
    public static UIElements Instance;
    [SerializeField] private Slider     healthBar;
    [SerializeField] private TMP_Text   healthIndex;
    [SerializeField] private Slider     expBar;
    [SerializeField] private TMP_Text   levelIndex;
    [SerializeField] public  CellSlider resourceBar;

    private void Awake()
    {
        Instance ??= this;
    }

    private int _cachedMaxHealth = -1;

    public void SetHealth(int _currentHealth = -1, int _maxHealth = -1)
    {
        if (_cachedMaxHealth == -1) _cachedMaxHealth = GameManager.Instance.GetLocalPlayer().Stats.MaxHealth;

        if (_maxHealth == -1) //최대 체력이 -1이면 기존 최대 체력으로 설정
            _maxHealth = _cachedMaxHealth;
        else //아니면 최대 체력 갱신
            healthBar.maxValue = _cachedMaxHealth = _maxHealth;

        if (_currentHealth == -1) //현재 체력이 -1이면 기존 현재 체력으로 설정
            _currentHealth = (int)healthBar.value;
        else //아니면 현재 체력 갱신
            healthBar.value = _currentHealth;

        //체력바 텍스트 갱신
        healthIndex.text = $"{_currentHealth:0}/{_maxHealth:0}";
    }

    public void SetExp(int    _currentExp) => expBar.value = _currentExp;
    public void SetMaxExp(int _maxExp)     => expBar.maxValue = _maxExp;

    public void SetLevelIndex(int _level) => levelIndex.text = $"{_level} Level";
    
    // public void Notified(NotifyTypes _type, object _value)
    // {
    //     switch (_type)
    //     {
    //         case NotifyTypes.Health:
    //             SetHealth(_currentHealth: (int)_value);
    //             break;
    //         case NotifyTypes.MaxHealth:
    //             SetHealth(_maxHealth: (int)_value);
    //             break;
    //         case NotifyTypes.Exp:
    //             SetExp((int)_value);
    //             break;
    //         case NotifyTypes.Level:
    //             SetLevelIndex((int)_value);
    //             break;
    //
    //     }
    // }
}