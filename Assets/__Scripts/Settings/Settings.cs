using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
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
    public struct ResolutionValue
    {
        public readonly int width, height;

        public ResolutionValue(Resolution _resolution)
        {
            width  = _resolution.width;
            height = _resolution.height;
        }

        public override string ToString()
        {
            return $"{width}x{height}";
        }

        public void ApplyResolution(FullScreenMode _fullScreenMode)
        {
            Screen.SetResolution(width, height, _fullScreenMode);
        }
    }

    [Serializable]
    public class General
    {
        private int _localizationIndex;

        public int LocalizationIndex
        {
            get => _localizationIndex;
            set
            {
                _localizationIndex                  = value;
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[value];
            }
        }
    }

    [Serializable]
    public class Graphic
    {
        private bool           _useVsync;
        private FullScreenMode _fullScreenMode;
        private int            _resolutionIndex;
        private int            _frameRate;

        private static List<ResolutionValue> _resolutionList;

        public static void Init()
        {
            _resolutionList = Screen.resolutions.Select(_res => new ResolutionValue(_res)).Distinct().ToList();
        }

        public static List<ResolutionValue> ResolutionList => _resolutionList;

        public int ResolutionIndex
        {
            get => _resolutionIndex;
            set
            {
                bool _doNotUpdateResolution = _resolutionIndex == value;

                _resolutionIndex = value;
                if (_doNotUpdateResolution)
                    return;

                ResolutionValue _resolution = ResolutionList[value];
                _resolution.ApplyResolution(Screen.fullScreenMode);

                EventBus.Publish(new ResolutionChanged(_resolution));
            }
        }

        public int FrameRate
        {
            get => _frameRate;
            set
            {
                _frameRate                  = value;
                Application.targetFrameRate = Settings.GetRefreshRateByIndex(value);
            }
        }

        public FullScreenMode FullScreenMode
        {
            get => _fullScreenMode;
            set
            {
                _fullScreenMode       = value;
                Screen.fullScreenMode = value;
            }
        }

        public bool UseVsync
        {
            get => _useVsync;
            set
            {
                _useVsync                  = value;
                QualitySettings.vSyncCount = value ? 1 : 0;
            }
        }
    }

    [Serializable]
    public class Sound
    {
        [SerializeField] [JsonIgnore] private AudioMixer _audioMixer;

        [JsonIgnore] public AudioMixer AudioMixer
        {
            get
            {
                _audioMixer ??= Addressables.LoadAssetAsync<AudioMixer>("Assets/Audio.mixer").WaitForCompletion();
                return _audioMixer;
            }
        }

        public float _masterVolume;
        public float _bgmVolume;
        public float _sfxVolume;

        public float MasterVolume
        {
            get => _masterVolume;
            set
            {
                _masterVolume = value;

                AudioMixer.SetFloat("Master", GetLogVolume(value));
            }
        }

        public float BGMVolume
        {
            get => _bgmVolume;
            set
            {
                _bgmVolume = value;
                AudioMixer.SetFloat("BGM", GetLogVolume(value));
            }
        }

        public float SFXVolume
        {
            get => _sfxVolume;
            set
            {
                _sfxVolume = value;
                AudioMixer.SetFloat("SFX", GetLogVolume(value));
            }
        }

        private float GetLogVolume(float _volume)
        {
            return _volume > 0 ? Mathf.Log10(_volume) * 20 : -80;
        }

        public void ApplyVolume()
        {
            AudioMixer.SetFloat("Master", GetLogVolume(MasterVolume));
            AudioMixer.SetFloat("BGM",    GetLogVolume(BGMVolume));
            AudioMixer.SetFloat("SFX",    GetLogVolume(SFXVolume));
        }
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
        _settingData.sound.ApplyVolume();
    }

    public static void Save(SettingData _settingData) =>
        File.WriteAllText($"{Application.persistentDataPath}/Settings.json", JsonConvert.SerializeObject(_settingData));
}