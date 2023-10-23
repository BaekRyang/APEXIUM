using UnityEngine;

public class GlobalInstaller :MonoBehaviour
{
    public SettingData settingData = SettingData.Load();
    public void Awake()
    {
        DIContainer.Global.Register(settingData);
    }
    
}