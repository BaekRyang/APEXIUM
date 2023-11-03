using UnityEngine;

public class GlobalInstaller : MonoBehaviour
{
    public SettingData settingData;
    public Settings   settings;

    public void Awake()
    {
        settingData = SettingData.Load();
        DIContainer.Global.Register(settingData);
        DIContainer.Global.Register(settings);
        DIContainer.Global.Register(new PlayData());
    }
}