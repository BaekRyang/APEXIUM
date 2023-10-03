using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BossPlayMap : PlayMap
{
    [SerializeField] private Tilemap bossRoomTilemap;
    [SerializeField] private Vector2 entrancePositionOffset;

    public Vector2 GetEntranceOffset => entrancePositionOffset;

    public void ProcessBossRoom()
    {
        bossRoomTilemap = GetComponentInChildren<Tilemap>();

        BossRoomEntrance _bossRoomEntrance = GetComponentInChildren<BossRoomEntrance>();

        if (_bossRoomEntrance == null) return;
        entrancePositionOffset = (Vector2)_bossRoomEntrance.transform.position //문의 위치(월드좌표)
                               - (Vector2)transform.position                   //맵의 위치(월드좌표)
                               + new Vector2(0, GetSize.y);                    //해당 오프셋은 맵의 피벗인 왼쪽 위 기준이므로
                                                                               //왼쪽 아래 기준으로 바꿔줘야함 (맵의 높이만큼 더해줌)
    }
}