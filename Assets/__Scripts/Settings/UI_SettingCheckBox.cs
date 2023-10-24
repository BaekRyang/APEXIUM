using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
            SettingValueBoolean.UseVsync => _settingData.graphic.useVsync,
            _                            => throw new ArgumentOutOfRangeException()
        };

    public void SetValue(bool _value)
    {
        switch (settingValueBoolean)
        {
            case SettingValueBoolean.UseVsync:
                _settingData.graphic.useVsync = _value;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void Initialize()
    {
        settingNameText.text = settingValueBoolean.ToString();

        Toggle _toggle = GetComponent<Toggle>();

        //값을 소수점 2째자리까지만 표시하고 설정에 업데이트
        _toggle.onValueChanged.AddListener(SetValue);
        _toggle.isOn = GetValue();
    }
}