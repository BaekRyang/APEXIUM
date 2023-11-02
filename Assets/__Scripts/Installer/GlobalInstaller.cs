using UnityEngine;

public class GlobalInstaller : MonoBehaviour
{
    public SettingData settingData;

    public void Awake()
    {
        settingData = SettingData.Load();
        DIContainer.Global.Register(settingData);
        DIContainer.Global.Register(new PlayData());
    }
}