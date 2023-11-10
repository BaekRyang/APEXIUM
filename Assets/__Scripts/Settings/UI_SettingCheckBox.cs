using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using Screen = UnityEngine.Device.Screen;

[RequireComponent(typeof(Toggle))]
public class UI_SettingCheckBox : DIMono
{
    [Inject]         private SettingData         _settingData;
    [SerializeField] private SettingValueBoolean settingValueBoolean;

    [SerializeField] private TMP_Text settingNameText;

    public enum SettingValueBoolean
    {
        UseVsync
    }

    public bool GetValue() => settingValueBoolean switch
    {
        SettingValueBoolean.UseVsync => _settingData.graphic.UseVsync,
        _                            => throw new ArgumentOutOfRangeException()
    };

    public void SetValue(bool _value)
    {
        switch (settingValueBoolean)
        {
            case SettingValueBoolean.UseVsync:
                _settingData.graphic.UseVsync = _value;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void Initialize()
    {
        settingNameText.text = settingValueBoolean.ToString();
        LocalizeStringEvent _localizeStringEvent = settingNameText.GetComponent<LocalizeStringEvent>();
        _localizeStringEvent.SetTable($"Settings");
        _localizeStringEvent.SetEntry($"{settingValueBoolean}");

        Toggle _toggle = GetComponent<Toggle>();

        //값을 소수점 2째자리까지만 표시하고 설정에 업데이트
        _toggle.onValueChanged.AddListener(SetValue);
        _toggle.isOn = GetValue();
    }
}