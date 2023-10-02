using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

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
    [SerializeField] private PlayMap     currentMap;
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

    public static Vector2 GetRandomPositionInMap(MapData _mapData)
    {
        float _yOffset = _mapData.currentMap.GetMapSize().y;
        var   _points  = _mapData.currentMap.GetComponent<PolygonCollider2D>().points;

        var _rightTopPoint = _points.OrderByDescending(_point => _point.x)
                                    .ThenByDescending(_point => _point.y)
                                    .First();

        var _leftTopPoint = _points.OrderBy(_point => _point.x)
                                   .ThenByDescending(_point => _point.y)
                                   .First();


        var _randomTopPoint = new Vector2(UnityEngine.Random.Range(_rightTopPoint.x, _leftTopPoint.x), _rightTopPoint.y + _yOffset);
        var _everyContactPoints = GetEveryContactPoints(_randomTopPoint);
        Vector2 _randomPoint = _everyContactPoints[UnityEngine.Random.Range(0, _everyContactPoints.Count)];
        return _randomPoint;
    }

    public static List<Vector2> GetEveryContactPoints(Vector2 _randomTopPoint)
    {
        Vector2       rayStartPoint  = _randomTopPoint;
        List<Vector2> _contactPoints = new List<Vector2>();
        int           loopCnt        = 0;
        while (true)
        {
            loopCnt++;
            if (loopCnt > 100)
            {
                Debug.LogError("Loop count exceeded");
                break;
            }

            //레이를 
            var _randomPoints = Physics2D.Raycast(rayStartPoint, Vector2.down, 1000, LayerMask.GetMask("Floor"));
            if (_randomPoints.collider == null)
                break;

            Debug.Log($"Picked Point = {_randomPoints.point}");

            _contactPoints.Add(_randomPoints.point);

            float cellHeight = 1;
            int   loopCnt2   = 0;
            rayStartPoint = _randomPoints.point - new Vector2(0, cellHeight);
            while (Physics2D.OverlapPoint(rayStartPoint, LayerMask.GetMask("Floor")) != null)
            {
                loopCnt2++;
                if (loopCnt2 > 100)
                {
                    Debug.LogError("Loop count2 exceeded");
                    break;
                }

                rayStartPoint -= new Vector2(0, cellHeight);
            }
        }

        return _contactPoints;
    }
}