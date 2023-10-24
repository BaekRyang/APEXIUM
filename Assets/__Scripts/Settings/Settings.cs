using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [Inject] private SettingData   _settingData;
    [Inject] private CameraManager _cameraManager;

    private void Start()
    {
        DIContainer.Inject(this);
        SR();
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
        SettingData.Save(_settingData);
    }

    public void SR()
    {
        SetResolution(Screen.width, Screen.height);
    }

    public void SetResolution(int _width, int _height)
    {
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
}

[Serializable]
public class SettingData
{
    public Graphic graphic = new();
    public Sound   sound   = new();

    [Serializable]
    public class Graphic
    {
        public bool useVsync;
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

    public static void Save(SettingData _settingData) =>
        File.WriteAllText($"{Application.persistentDataPath}/Settings.json", JsonConvert.SerializeObject(_settingData));
}