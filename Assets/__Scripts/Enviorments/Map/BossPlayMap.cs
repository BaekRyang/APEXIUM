using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BossPlayMap : PlayMap
{
    [SerializeField] private Tilemap bossRoomTilemap;


    public void ProcessBossRoom()
    {
        bossRoomTilemap = GetComponentInChildren<Tilemap>();
    }
}