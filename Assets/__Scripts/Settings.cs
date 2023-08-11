using System;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    
    /// <summary>
    /// PPU와 Height Resolution을 통해 Orthographic Size를 계산
    /// </summary>
    public static float CalculateOrthographicSize(float p_ppu, float p_height)
    {
        float _orthographicSize = p_height / 2f / p_ppu;
        return _orthographicSize;
    }
}