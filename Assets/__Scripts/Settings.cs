using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public RawImage[] rawImages;

    /// <summary>
    /// PPU와 Height Resolution을 통해 Orthographic Size를 계산
    /// </summary>
    private static float CalculateOrthographicSize(float _ppu, float _height)
    {
        float _orthographicSize = _height / 2f / _ppu;
        return _orthographicSize;
    }

    public void SR()
    {
        SetResolution(Screen.width, Screen.height, Camera.main.GetComponent<CinemachineVirtualCamera>());
    }

    public void SetResolution(int _width, int _height, CinemachineVirtualCamera _camera)
    {
        //TODO: 코드 정리 필요
        Debug.Log($"<color=green>SetResolution</color> : {_width}x{_height}");
        var _array = Camera.allCameras
                           .Select(_cam => _cam.GetComponent<Camera>())
                           .Where(_cam => _cam.name.StartsWith("T_"))
                           .ToArray();


        for (int _index = 0; _index < _array.Length; _index++)
        {
            Camera _cams = _array[_index];

            if (_cams.targetTexture != null)
            {
                _cams.targetTexture.Release();
                Destroy(_cams.targetTexture);
            }

            RenderTexture _targetTexture = new(_width, _height, 16, RenderTextureFormat.ARGBFloat); //HDR 사용(Float)

            rawImages[_index].texture = _targetTexture;

            _targetTexture.Create();

            _cams.targetTexture = _targetTexture;
        }
    }
}