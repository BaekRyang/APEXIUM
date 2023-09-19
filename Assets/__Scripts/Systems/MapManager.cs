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
}