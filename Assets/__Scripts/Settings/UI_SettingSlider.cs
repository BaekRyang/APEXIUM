using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UI_SettingSlider : DIMono
{
    [Inject]         private SettingData       _settingData;
    [SerializeField] private SettingValueFloat settingValueFloat;
    [SerializeField] private TMP_Text          settingNameText;
    [SerializeField] private TMP_Text          labelText;

    public enum SettingValueFloat
    {
        MasterVolume,
        BGMVolume,
        SFXVolume
    }

    public float GetValue() => settingValueFloat switch
    {
        SettingValueFloat.MasterVolume => _settingData.sound.masterVolume,
        SettingValueFloat.BGMVolume    => _settingData.sound.bgmVolume,
        SettingValueFloat.SFXVolume    => _settingData.sound.sfxVolume,
        _                              => 0
    };

    public void SetValue(float _value)
    {
        switch (settingValueFloat)
        {
            case SettingValueFloat.MasterVolume:
                _settingData.sound.masterVolume = _value;
                break;
            case SettingValueFloat.BGMVolume:
                _settingData.sound.bgmVolume = _value;
                break;
            case SettingValueFloat.SFXVolume:
                _settingData.sound.sfxVolume = _value;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        labelText.text = $"{_value * 100:0}";
    }

    public override void Initialize()
    {
        settingNameText.text = settingValueFloat.ToString();
        LocalizeStringEvent _localizeStringEvent = settingNameText.GetComponent<LocalizeStringEvent>();
        _localizeStringEvent.SetTable($"Settings");
        _localizeStringEvent.SetEntry($"{settingValueFloat}");
        
        Slider _slider = GetComponent<Slider>();

        //값을 소수점 2째자리까지만 표시하고 설정에 업데이트
        _slider.onValueChanged.AddListener(_f =>
        {
            float _roundedValue = Mathf.Round(_f * 100) / 100;
            _slider.value = _roundedValue;
            SetValue(_roundedValue);
        });
        _slider.value = GetValue();
    }
}