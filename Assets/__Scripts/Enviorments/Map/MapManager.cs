using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    [SerializeField] private PlayMap currentMap, bossPlayMap;

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
        currentMap = _playMapData.mapData[0].currentMap;

        if (_playMapData.mapData.Length > 1)
            bossPlayMap = _playMapData.mapData[1].currentMap;
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
        return GetSpawnLocation(currentMap);
    }
    
    public static Vector2 GetSpawnLocation(PlayMap _currentMap)
    {
        Vector2 _targetPosition;
        // {
        //     //TODO : 임시로 맵 중앙에 스폰
        //     _mapCenter = _currentMap.GetSize / 2;
        // }
        {
            _targetPosition =  GetRandomPositionInMap(_currentMap, new Vector2(10, 10));
        }
        
        Vector2 _spawnPosition = Physics2D.RaycastAll(_targetPosition, Vector2.down, 200, LayerMask.GetMask("Floor"))[^1].point;
        return _spawnPosition;
    }

    public static Vector2 GetRandomPositionInMap(PlayMap _mapData, Vector2 _padding)
    {
        float _yOffset = _mapData.GetMapSize().y;
        var   _points  = _mapData.GetComponent<PolygonCollider2D>().points;

        var _rightTopPoint = _points.OrderByDescending(_point => _point.x)
                                    .ThenByDescending(_point => _point.y)
                                    .First();

        var _leftTopPoint = _points.OrderBy(_point => _point.x)
                                   .ThenByDescending(_point => _point.y)
                                   .First();

        Vector2 _randomTopPoint = new(
            x: Random.Range(_rightTopPoint.x - _padding.x, _leftTopPoint.x + _padding.x),
            y: _rightTopPoint.y - _padding.y + _yOffset);

        var     _everyContactPoints = GetEveryContactPoints(_randomTopPoint);
        Vector2 _randomPoint        = _everyContactPoints[Random.Range(0, _everyContactPoints.Count)];

        Debug.DrawLine(_randomTopPoint, _randomPoint, Color.red, 10f);
        return _randomPoint;
    }

    public static Vector2 GetRandomPositionNearPlayer(Player _player)
    {
        if (_player is null)
            return Vector2.zero;

        Vector3 _position = _player.transform.position;

        float _yOffset = _position.y;

        Vector2 _rightTopPoint = new(_position.x + 20, _position.y + 15);
        Vector2 _leftTopPoint  = new(_position.x - 20, _position.y + 15);

        Vector2 _randomTopPoint = new(Random.Range(_rightTopPoint.x, _leftTopPoint.x), _rightTopPoint.y + _yOffset);

        var     _everyContactPoints = GetEveryContactPoints(_randomTopPoint, 40);
        Vector2 _randomPoint        = _everyContactPoints[Random.Range(0, _everyContactPoints.Count)];

        Debug.DrawLine(_randomTopPoint, _randomPoint, Color.green, 10f);
        return _randomPoint;
    }

    private const float CELL_HEIGHT = 1;

    private static List<Vector2> GetEveryContactPoints(Vector2 _randomTopPoint, int _maxDepth = -1)
    {
        Vector2 _rayStartPoint = _randomTopPoint;
        var     _contactPoints = new List<Vector2>();
        int     _loopCnt       = 0;
        int     _currentDepth  = 0;

        while (true)
        {
            if (Tools.LoopLimit(ref _loopCnt))
                break;

            RaycastHit2D _randomPoints = Physics2D.Raycast(
                _rayStartPoint,
                Vector2.down,
                1000,
                LayerMask.GetMask("Floor")); //레이를 처음 위치에서 쏜다.

            Debug.DrawLine(_randomPoints.point - Vector2.left, _randomPoints.point + Vector2.right, Color.cyan, 10f);

            if (_randomPoints.collider == null) //null이면 맵 바깥(여기에서는 최하단 아래)에서 쏜 것
                break;

            _contactPoints.Add(_randomPoints.point); //null이 아니면 해당 포인트를 리스트에 저장하고

            int _loopCnt2 = 0;
            _rayStartPoint = _randomPoints.point - new Vector2(0, CELL_HEIGHT);                //해당 포인트 1타일 아래
            while (Physics2D.OverlapPoint(_rayStartPoint, LayerMask.GetMask("Floor")) != null) //해당 위치에 타일이 있는가?
            {
                if (Tools.LoopLimit(ref _loopCnt2))
                    break;

                if (_maxDepth != -1 && _currentDepth >= _maxDepth)
                    break;

                _currentDepth++;
                _rayStartPoint -= new Vector2(0, CELL_HEIGHT); //있다면 한 칸 아래로 변경 ->

                //while을 통해서 또 조사하고 없다면 Ray를 쏴서 Point를 찾는다.
            }
        }

        return _contactPoints;
    }

    public static bool HasTile(Tilemap _tilemap, Vector3 _targetPosition)
    {
        //위치를 타일맵의 로컬 좌표로 변환한다.
        Vector3 _localPosition = _tilemap.transform.InverseTransformPoint(_targetPosition);

        Vector3Int _tilePosition = new(Mathf.FloorToInt(_localPosition.x), Mathf.FloorToInt(_localPosition.y));

        return _tilemap.HasTile(_tilePosition);
    }
}