using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
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

    public void SR()
    {
        SetResolution(Screen.width, Screen.height);
    }

    public void SetResolution(int _width, int _height)
    {
        //TODO: 코드 정리 필요

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
            _targetTexture.name = "TransitionTexture " + (_index + 1);

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