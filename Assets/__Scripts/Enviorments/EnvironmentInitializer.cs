using System;
using System.Collections;
using MoreMountains.Feedbacks;
using UnityEditor;
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
    private                  MapTheme?  _currentTheme;
    [SerializeField] private GameObject bossRoomEntrance;
    [Inject]         private Settings   settings;

    private const string DATA_DIRECTORY = "Datasets";

    private void Start()
    {
        (MapData _playMap, MapData _bossMap) = EscalateMap();
        _playMap.GetComponent<MMF_Player>().PlayFeedbacks();
        _bossMap.GetComponent<MMF_Player>().PlayFeedbacksInReverse();

        StartCoroutine(CameraSettings());
        
        EventBus.Publish(new PlayMapChangedEvent(_playMap, _bossMap));
    }

    private IEnumerator CameraSettings()
    {
        yield return new WaitForSeconds(.1f);
        
        DIContainer.Inject(this);

        SettingData.ResolutionValue _resolution = SettingData.Graphic.ResolutionList[settings.settingData.graphic.ResolutionIndex];
        settings.SetTransitionCameraResolution(_resolution.width, _resolution.height);
        settings.gameObject.SetActive(false);
    }

    //기본 배치 개수
    private const int MAX_CHEST_COUNT   = 20;
    private const int MAX_CAPSULE_COUNT = 10;
    private const int MAX_ALTER_COUNT   = 10;

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
        _normalMap.currentMap.SetEntranceOffset();
        PlaceBossRoomEntrance(_normalMap);
        PlaceInteractableObjects(_normalMap);

        MapData _bossMap = LoadMap((MapTheme)_currentTheme, MapType.Boss);
        _bossMap.currentMap.SetEntranceOffset();
        _bossMap.transform.root.position = //기존 맵 왼쪽에 붙여준다. (너무 가까우면 카메라 트랜지션에 보일 수있으므로 약간 떨어뜨림)
            new Vector3(-(_bossMap.currentMap.GetSize.x + 5), 0, 0);

        return (_normalMap, _bossMap);
    }

    private MapData LoadMap(MapTheme _theme, MapType _mapType)
    {
        string _mapDirectory = $"{DATA_DIRECTORY}/MapData/{_mapType}/{_theme}";
        Debug.Log(_mapDirectory);
        var _mapData = Resources.LoadAll<GameObject>(_mapDirectory);
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

    private void PlaceBossRoomEntrance(MapData _mapObject)
    {
        int     _loopCnt       = 0;
        Vector2 _outlineOffset = new() { x = 10, y = 10 };

        Vector2 _entranceSize = bossRoomEntrance.GetComponent<BoxCollider2D>().size;
        do
        {
            if (Tools.LoopLimit(ref _loopCnt)) break;

            Vector2 _randomPositionInMap = MapManager.GetSpawnLocation(_mapObject.currentMap);

            // Vector2 _randomPositionInMap = MapManager.GetRandomPositionInMap(_mapObject, _outlineOffset);

            if (_randomPositionInMap.y < 10)
            {
                Debug.Log("Too low");
                continue;
            }

            Collider2D _capturedCollider
                = Physics2D.OverlapBox(_randomPositionInMap + new Vector2(0, _entranceSize.y / 2 + .1f),
                                       _entranceSize,
                                       0,
                                       LayerMask.GetMask("Floor"));
            if (_capturedCollider != null)
            {
                Debug.Log("Overlap");
                continue;
            }

            //문의 양끝점 아래가 바닥에 닿아있는지 확인
            RaycastHit2D _leftConor = Physics2D.Raycast(_randomPositionInMap + Vector2.left * (_entranceSize.x / 2),
                                                        Vector2.down,
                                                        .1f,
                                                        LayerMask.GetMask("Floor"));
            Debug.DrawRay(_randomPositionInMap + Vector2.left * (_entranceSize.x / 2), Vector2.down * 1f, Color.blue, 1f);

            RaycastHit2D _rightConor = Physics2D.Raycast(_randomPositionInMap + Vector2.right * (_entranceSize.x / 2),
                                                         Vector2.down,
                                                         .1f,
                                                         LayerMask.GetMask("Floor"));
            Debug.DrawRay(_randomPositionInMap + Vector2.right * (_entranceSize.x / 2), Vector2.down * 1f, Color.blue, 1f);

            if (_leftConor.collider == null || _rightConor.collider == null)
            {
                Debug.Log("Not on the floor");
                continue;
            }

            Debug.Log($"Random Position : {_randomPositionInMap}");
            _mapObject.currentMap.bossRoomEntrance = Instantiate(bossRoomEntrance,
                                                                 _randomPositionInMap,
                                                                 Quaternion.identity,
                                                                 _mapObject.sceneObjects).transform;
            break;
        } while (true);
    }

    private GameObject PlaceObject(MapData _mapObject, GameObject _gameObject)
    {
        int     _loopCnt       = 0;
        Vector2 _outlineOffset = new() { x = 3, y = 10 };
        Vector2 _objectSize    = _gameObject.GetComponent<BoxCollider2D>().size;
        do
        {
            if (Tools.LoopLimit(ref _loopCnt)) break;

            Vector2 _randomPositionInMap = MapManager.GetRandomPositionInMap(_mapObject, _outlineOffset);

            if (_randomPositionInMap.y < 10)
            {
                Debug.Log(_gameObject.name + "Too low");
                continue;
            }

            Collider2D _capturedCollider
                = Physics2D.OverlapBox(_randomPositionInMap + new Vector2(0, _objectSize.y / 2 + .1f),
                                       _objectSize,
                                       0,
                                       LayerMask.GetMask("Floor", "Interactable"));
            if (_capturedCollider != null)
            {
                Debug.Log(_gameObject.name + "Floor Overlap");
                continue;
            }

            //오브젝트의 양 끝점 아래가 바닥에 닿아있는지 확인
            RaycastHit2D _left = Physics2D.Raycast(_randomPositionInMap + Vector2.left * (_objectSize.x / 2),
                                                   Vector2.down,
                                                   .1f,
                                                   LayerMask.GetMask("Floor"));
            RaycastHit2D _right = Physics2D.Raycast(_randomPositionInMap + Vector2.right * (_objectSize.x / 2),
                                                    Vector2.down,
                                                    .1f,
                                                    LayerMask.GetMask("Floor"));

            if (_left.collider == null || _right.collider == null)
            {
                Debug.Log($"{_gameObject.name}Not on the floor");
                continue;
            }

            Debug.Log($"{_gameObject.name}Random Position : {_randomPositionInMap}");

            return Instantiate(_gameObject,
                               _randomPositionInMap,
                               Quaternion.identity,
                               _mapObject.sceneObjects);
        } while (true);

        return null;
    }

    private void PlaceInteractableObjects(MapData _normalMap)
    {
        PlaceAlters(_normalMap);
        PlaceChests(_normalMap);
        PlaceCapsules(_normalMap);
    }

    private void PlaceAlters(MapData _normalMap) { }

    private void PlaceChests(MapData _normalMap)
    {
        for (int _i = 0; _i < MAX_CHEST_COUNT; _i++)
        {
            ChestType  _chest    = _normalMap.GetRandomChest();
            GameObject _chestObj = _normalMap.GetRandomChestGameObject(_chest);

            PlaceObject(_normalMap, _chestObj).GetComponent<ItemBox>().chestType = _chest;
        }
    }

    private void PlaceCapsules(MapData _normalMap)
    {
        GameObject _capsule = _normalMap.objectPrefabs.capsule;
        for (int _i = 0; _i < MAX_CAPSULE_COUNT; _i++)
            PlaceObject(_normalMap, _capsule);
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