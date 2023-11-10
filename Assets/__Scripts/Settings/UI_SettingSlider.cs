using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UI_SettingSlider : DIMono, IDeselectHandler, ISelectHandler
{
    [Inject]         private SettingData       _settingData;
    [SerializeField] private SettingValueFloat settingValueFloat;
    [SerializeField] private TMP_Text          settingNameText;
    [SerializeField] private TMP_Text          labelText;
    private                  Slider            _slider;
    private                  Navigation        _copyNavigation;

    public enum SettingValueFloat
    {
        MasterVolume,
        BGMVolume,
        SFXVolume
    }

    public float GetValue() => settingValueFloat switch
    {
        SettingValueFloat.MasterVolume => _settingData.sound.MasterVolume,
        SettingValueFloat.BGMVolume    => _settingData.sound.BGMVolume,
        SettingValueFloat.SFXVolume    => _settingData.sound.SFXVolume,
        _                              => 0
    };

    public void SetValue(float _value)
    {
        switch (settingValueFloat)
        {
            case SettingValueFloat.MasterVolume:
                _settingData.sound.MasterVolume = _value;
                break;
            case SettingValueFloat.BGMVolume:
                _settingData.sound.BGMVolume = _value;
                break;
            case SettingValueFloat.SFXVolume:
                _settingData.sound.SFXVolume = _value;
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
        
        _slider = GetComponent<Slider>();

        //값을 소수점 2째자리까지만 표시하고 설정에 업데이트
        _slider.onValueChanged.AddListener(_f =>
        {
            float _roundedValue = Mathf.Round(_f * 100) / 100;
            _slider.value = _roundedValue;
            SetValue(_roundedValue);
        });
        _slider.value = GetValue();
    }

    public void OnSelect(BaseEventData eventData)
    {
        Navigation _tmpNav = _slider.navigation;
        _copyNavigation = _tmpNav;

        _tmpNav.selectOnLeft = _tmpNav.selectOnRight = null;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _slider.navigation = _copyNavigation;
    }
}