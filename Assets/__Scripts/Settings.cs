using System;
using System.Threading.Tasks;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Settings : MonoBehaviour
{
    public TMP_InputField     widthInputField;
    public TMP_InputField     heightInputField;
    public TMP_InputField     ppuInputField;
    public PixelPerfectCamera pixelPerfectCamera;

    /// <summary>
    /// PPU와 Height Resolution을 통해 Orthographic Size를 계산
    /// </summary>
    public static float CalculateOrthographicSize(float p_ppu, float p_height)
    {
        float _orthographicSize = p_height / 2f / p_ppu;
        return _orthographicSize;
    }

    public static void SetResolution(int p_width, int p_height)
    {
        Screen.SetResolution(p_width, p_height, Screen.fullScreen);
    }

    public void SetResolution()
    {
        Debug.Log("SetResolution");
        SetResolution(int.Parse(widthInputField.text), int.Parse(heightInputField.text));
        pixelPerfectCamera.assetsPPU = int.Parse(ppuInputField.text);
        var cinemachineCamera = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera as CinemachineVirtualCamera;
        cinemachineCamera.m_Lens.OrthographicSize= CalculateOrthographicSize(pixelPerfectCamera.assetsPPU, int.Parse(heightInputField.text));
        cinemachineCamera.GetComponent<CinemachineConfiner2D>().InvalidateCache(); //CinemachineConfiner2D의 캐시를 갱신한다.


    }
}