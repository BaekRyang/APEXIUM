using System;
using System.Collections.Generic;
using UnityEngine;

public enum MapType
{
    Normal,
    Boss
}

public enum MapTheme
{
    Spring,
    Summer,
    Fall,
    Winter
}

/// <summary>
/// 클래스들이 맵 데이터에 접근할때 사용하는 클래스
/// </summary>
public class MapManager : MonoBehaviour
{
    [SerializeField] private PlayMap  currentMap;
    [SerializeField] private BossPlayMap bossPlayMap;

    private void OnEnable()
    {
        EventBus.Subscribe<PlayMapChangedEvent>(OnMapChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayMapChangedEvent>(OnMapChanged);
    }

    private void OnMapChanged(PlayMapChangedEvent _playMapData)
    {
        Debug.Log("<color=green>Map changed</color> : " + _playMapData.mapData.Length);
        currentMap = _playMapData.mapData[0].currentMap as PlayMap;

        if (_playMapData.mapData.Length > 1)
        {
            bossPlayMap = (_playMapData.mapData[1].currentMap as BossPlayMap)!.ProcessBossRoom();
        }
    }

    public PlayMap GetMap(MapType _mapType)
    {
        return _mapType switch
        {
            MapType.Normal => currentMap,
            MapType.Boss   => bossPlayMap,
            _              => null
        };
    }

    public Vector2 GetSpawnLocation()
    {
        //TODO : 임시로 맵 중앙에 스폰
        Vector2 _mapCenter     = currentMap.GetSize / 2;
        Vector2 _spawnPosition = Physics2D.RaycastAll(_mapCenter, Vector2.down, 200, LayerMask.GetMask("Floor"))[^1].point;
        _spawnPosition.y += 1.5f;

        return _spawnPosition;
    }
}