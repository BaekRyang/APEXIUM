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
    [SerializeField] private Transform sceneObjects;
    [SerializeField] private Transform background;
    [SerializeField] private PlayMap   currentMap;
    [SerializeField] private BossRoom  bossRoom;

    private void OnEnable()
    {
        EventBus.Subscribe<MapChangedEvent>(OnMapChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<MapChangedEvent>(OnMapChanged);
    }

    private void OnMapChanged(MapChangedEvent _mapData)
    {
        currentMap = _mapData.mapData.currentMap;
        background = _mapData.mapData.background;
        sceneObjects = _mapData.mapData.sceneObjects;
    }

    public PlayMap GetMap(MapType _mapType)
    {
        switch (_mapType)
        {
            case MapType.Normal:
                return currentMap;
            case MapType.Boss:
                return bossRoom;
            default:
                return null;    
        }
    }

    public Vector2 GetSpawnLocation()
    {
        //TODO : 임시로 맵 중앙에 스폰
        Vector2 _mapCenter = currentMap.GetSize / 2;
        Vector2 _spawnPosition = Physics2D.RaycastAll(_mapCenter, Vector2.down, 200, LayerMask.GetMask("Floor"))[^1].point;
        _spawnPosition.y += 1.5f;
        
        return _spawnPosition;
    }
}