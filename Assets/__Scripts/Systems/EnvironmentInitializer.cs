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
    private MapTheme? _currentTheme = null;

    private const string DATA_DIRECTORY = "Datasets";

    private void Start()
    {
        (MapData _playMap, MapData _bossMap) = EscalateMap();

        EventBus.Publish(new PlayMapChangedEvent(_playMap));
    }

    private MapData LoadMap(MapTheme _theme, MapType _mapType)
    {
        string       _mapDirectory    = DATA_DIRECTORY + "/MapData/" + _mapType + "/" + _theme;
        GameObject[] _mapData         = Resources.LoadAll<GameObject>(_mapDirectory);
        GameObject   _selectedMapData = _mapData[Random.Range(0, _mapData.Length)];

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

        return _currentTheme is null ?
            (null, null) : //TODO : 게임 클리어
            (LoadMap((MapTheme)_currentTheme, MapType.Normal), LoadMap((MapTheme)_currentTheme, MapType.Boss));
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