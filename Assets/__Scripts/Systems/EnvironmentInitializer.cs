using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 게임씬 처음 접근시, 보스 처치후 다음 맵으로 넘어갈때 사용하는
/// 게임 오브젝트를 리셋하는 클래스
/// 리소스에서 무작위 맵 데이터를 불러와서 게임 오브젝트를 생성하고
/// 이벤트 버스를 통해 맵 데이터를 전달해준다.
/// </summary>
public class EnvironmentInitializer : MonoBehaviour
{
    private                  MapTheme?  _currentTheme = null;
    [SerializeField] private GameObject bossRoomEntrance;

    private const string DATA_DIRECTORY = "Datasets";

    private void Start()
    {
        (MapData _playMap, MapData _bossMap) = EscalateMap();

        EventBus.Publish(new PlayMapChangedEvent(_playMap, _bossMap));
    }

    private MapData LoadMap(MapTheme _theme, MapType _mapType)
    {
        string _mapDirectory = DATA_DIRECTORY + "/MapData/" + _mapType + "/" + _theme;
        Debug.Log(_mapDirectory);
        GameObject[] _mapData = Resources.LoadAll<GameObject>(_mapDirectory);
        if (_mapData.Length == 0)
        {
            Debug.LogError("Map data not found");
            return null;
        }

        GameObject _selectedMapData = _mapData[Random.Range(0, _mapData.Length)];
        if (_selectedMapData != null)
        {
            GameObject _map = Instantiate(_selectedMapData);

            return _map.GetComponent<MapData>();
        }

        Debug.LogError("Map data not found");
        return null;
    }

    private (MapData, MapData) EscalateMap()
    {
        _currentTheme = _currentTheme switch
        {
            null            => MapTheme.Spring,
            MapTheme.Spring => MapTheme.Summer,
            MapTheme.Summer => MapTheme.Fall,
            MapTheme.Fall   => MapTheme.Winter,
            MapTheme.Winter => null,
            _               => throw new Exception("Map theme is not defined")
        };

        if (_currentTheme is null)
            return (null, null);


        MapData _normalMap = LoadMap((MapTheme)_currentTheme, MapType.Normal);
        PlaceObjects(_normalMap);


        MapData _bossMap = LoadMap((MapTheme)_currentTheme, MapType.Boss);
        PlaceObjects(_bossMap);

        Transform _bossMapTransform = _bossMap.transform.root;
        _bossMapTransform.gameObject.SetActive(false); //보스맵은 비활성화
        _bossMapTransform.position = Vector3.back;

        if (_bossMap.currentMap is BossPlayMap _bossPlayMap)
        {
            _bossPlayMap.ProcessBossRoom();

            //보스방 위치를 보스방의 문과 맵의 문을 찾아서 맞추는 작업
            Vector3 _playMapDoorPosition = _normalMap.bossRoomEntrance.position;
            Vector3 _bossMapDoorOffset   = _bossPlayMap.GetEntranceOffset;

            _bossMapTransform.position = _playMapDoorPosition - _bossMapDoorOffset;
        }

        return (_normalMap, _bossMap);
    }

    private void PlaceObjects(MapData _mapObject)
    {
        if (_mapObject.currentMap is not BossPlayMap) //보스맵에는 이미 입구가 생성되어 있음
            PlaceBossRoomEntrance(_mapObject);
    }

    private void PlaceBossRoomEntrance(MapData _mapObject)
    {
        int     _loopCnt      = 0;
        Vector2 _entranceSize = bossRoomEntrance.GetComponent<BoxCollider2D>().size;
        do
        {
            if (Tools.LoopLimit(ref _loopCnt)) break;

            Vector2 _randomPositionInMap = MapManager.GetRandomPositionInMap(_mapObject);
            Collider2D _capturedCollider
                = Physics2D.OverlapBox(_randomPositionInMap + new Vector2(0, _entranceSize.y / 2 + .1f),
                                       _entranceSize,
                                       0,
                                       LayerMask.GetMask("Floor"));
            if (_capturedCollider != null)
                continue;

            Debug.Log("Random Position : " + _randomPositionInMap);
            _mapObject.bossRoomEntrance = Instantiate(bossRoomEntrance,
                                                      _randomPositionInMap,
                                                      Quaternion.identity,
                                                      _mapObject.currentMap.transform).transform;
            break;
        } while (true);
    }
}

public class PlayMapChangedEvent
{
    public readonly MapData[] mapData;

    public PlayMapChangedEvent(params MapData[] _mapData)
    {
        mapData = _mapData;
    }
}