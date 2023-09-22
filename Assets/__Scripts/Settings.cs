using System;
using System.Threading.Tasks;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Settings : MonoBehaviour
{
    public PixelPerfectCamera pixelPerfectCamera;

    /// <summary>
    /// PPU와 Height Resolution을 통해 Orthographic Size를 계산
    /// </summary>
    private static float CalculateOrthographicSize(float _ppu, float _height)
    {
        float _orthographicSize = _height / 2f / _ppu;
        return _orthographicSize;
    }

    public void SetResolution(int _ppu, int _height, CinemachineVirtualCamera _camera)
    {
        pixelPerfectCamera.assetsPPU    = _ppu;
        _camera.m_Lens.OrthographicSize = CalculateOrthographicSize(pixelPerfectCamera.assetsPPU, _height);
        _camera.GetComponent<CinemachineConfiner2D>().InvalidateCache();
    }
}