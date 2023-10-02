using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BossPlayMap : PlayMap
{
    [SerializeField] private Tilemap _bossRoomTilemap;
    [SerializeField] private Vector2 _positionOffset;
    
    public BossPlayMap ProcessBossRoom()
    {
        _bossRoomTilemap = GetComponentInChildren<Tilemap>();
        
        Vector2 _bossRoomEntrancePosition = Vector2.zero;
        BossRoomEntrance _bossRoomEntrance = GetComponentInChildren<BossRoomEntrance>();
        
        if (_bossRoomEntrance == null) return this;
        _bossRoomEntrancePosition = _bossRoomEntrance.transform.position;
        _positionOffset = _bossRoomEntrancePosition - (Vector2)transform.position;
        return this;
    }
}
