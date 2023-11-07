using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [Inject] public  SettingData   settingData;
    [Inject] private CameraManager _cameraManager;
    
    private void Start()
    {
        DIContainer.Inject(this);
    }

    /// <summary>
    /// PPU와 Height Resolution을 통해 Orthographic Size를 계산
    /// </summary>
    private static float CalculateOrthographicSize(float _ppu, float _height)
    {
        float _orthographicSize = _height / 2f / _ppu;
        return _orthographicSize;
    }

    public void SaveSettingToFile()
    {
        SettingData.Save(settingData);
    }

    public void SetTransitionCameraResolution(int _width, int _height)
    {
        if (!_cameraManager) return;
        //TODO: 이거 여기서 하는게 아니라 카메라 매니저로 넘겨야 함
        
        for (int _index = 0; _index < 2; _index++)
        {
            Camera _cams = _cameraManager.transitionCameras[_index];

            RenderTexture _camsTargetTexture = _cams.targetTexture;

            if (_camsTargetTexture != null) //이미 렌더 텍스쳐가 있다면
            {
                _camsTargetTexture.Release(); //렌더 텍스쳐를 해제하고
                Destroy(_camsTargetTexture);  //렌더 텍스쳐를 삭제
            }

            RenderTexture _targetTexture = new(_width, _height, 16, RenderTextureFormat.ARGBFloat); //HDR 사용(Float)
            _targetTexture.name = $"TransitionTexture {_index + 1}";

            _cameraManager.transitionTexture[_index].texture = _targetTexture;

            _targetTexture.Create();

            _cams.targetTexture = _targetTexture;
        }
    }

    public void ChangeOrthographicSize(float _orthographicSize)
    {
        _cameraManager.mainVirtualCamera.m_Lens.OrthographicSize = _orthographicSize;
    }

    public static int GetRefreshRateByIndex(int _index) => _index switch
    {
        0 => 30,
        1 => 60,
        2 => 75,
        3 => 120,
        4 => 144,
        5 => 180,
        6 => 240,
        7 => 300,
        8 => -1,
        _ => throw new ArgumentOutOfRangeException()
    };
}

[Serializable]
public class SettingData
{
    public General general = new();
    public Graphic graphic = new();
    public Sound   sound   = new();

    [Serializable]
    public struct Resolution
    {
        public readonly int width, height;

        public Resolution(UnityEngine.Resolution res)
        {
            width  = res.width;
            height = res.height;
        }

        public override string ToString()
        {
            return $"{width}x{height}";
        }

        public void ApplyResolution(SettingData _settingData)
        {
            Screen.SetResolution(width, height, _settingData.graphic.fullScreenMode);
        }
    }

    [Serializable]
    public class General
    {
        public int LocalizationIndex;
    }

    [Serializable]
    public class Graphic
    {
        public bool           useVsync;
        public FullScreenMode fullScreenMode;
        public int            resolutionIndex;
        public int            frameRate;

        private static List<Resolution> _resolutionList;

        public static void Init()
        {
            _resolutionList = Screen.resolutions.Select(_res => new Resolution(_res)).Distinct().ToList();
        }

        public static List<Resolution> ResolutionList => _resolutionList;
    }

    [Serializable]
    public class Sound
    {
        public float masterVolume;
        public float bgmVolume;
        public float sfxVolume;
    }

    public static SettingData Load()
    {
        string      Path = $"{Application.persistentDataPath}/Settings.json";
        SettingData _settingData;
        if (!File.Exists(Path))
        {
            Debug.Log($"File not found : {Path} : Create new file");
            _settingData = new SettingData();
        }
        else
        {
            Debug.Log($"File found : {Path} : Load file");
            string _json = File.ReadAllText(Path);
            _settingData = JsonConvert.DeserializeObject<SettingData>(_json);
        }

        if (_settingData == null)
        {
            Debug.Log($"Data is null : {Path} : Create new file");
            _settingData = new SettingData();
        }

        Save(_settingData);

        return _settingData;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    public static void BeforeGameStart()
    {
        Graphic.Init();
        SettingData _settingData = Load();

        Graphic.ResolutionList[_settingData.graphic.resolutionIndex].ApplyResolution(_settingData);
        Application.targetFrameRate         = Settings.GetRefreshRateByIndex(_settingData.graphic.frameRate);
        QualitySettings.vSyncCount          = _settingData.graphic.useVsync ? 1 : 0;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_settingData.general.LocalizationIndex];

        //그래픽 세팅 불러오기
    }

    public static void Save(SettingData _settingData) =>
        File.WriteAllText($"{Application.persistentDataPath}/Settings.json", JsonConvert.SerializeObject(_settingData));
}