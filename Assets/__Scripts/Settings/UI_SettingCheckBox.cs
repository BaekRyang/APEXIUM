using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class UI_SettingCheckBox : DIMono
{
    [Inject] private SettingData _settingData;
    private          object      _settingValueBoolean;

    public enum SettingValueBoolean
    {
        UseVsync
    }

    public bool GetValue()
    {
        switch (_settingValueBoolean)
        {
            case SettingValueBoolean.UseVsync:
                return _settingData.graphic.useVsync;
            default:
                throw new System.ArgumentOutOfRangeException();
        }
    }
}