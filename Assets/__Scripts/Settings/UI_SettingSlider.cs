using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UI_SettingSlider : DIMono
{
    [Inject] private SettingData       _settingData;
    public           SettingValueFloat settingValueFloat;
    public           TMP_Text          indexText;

    public enum SettingValueFloat
    {
        BGMVolume,
        SFXVolume
    }

    public float GetValue()
    {
        switch (settingValueFloat)
        {
            case SettingValueFloat.BGMVolume:
                return _settingData.sound.volume;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetValue(float _value)
    {
        switch (settingValueFloat)
        {
            case SettingValueFloat.BGMVolume:
                _settingData.sound.volume = _value;
                indexText.text = $"{_value * 100:0}";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void Initialize()
    {
        Slider _slider = GetComponent<Slider>();
        _slider.onValueChanged.AddListener(SetValue);
        _slider.value = GetValue();
        
        indexText = transform.parent.Find("Index").GetComponent<TMP_Text>();
    }
}