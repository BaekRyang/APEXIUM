using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 게임씬 처음 접근시, 보스 처치후 다음 맵으로 넘어갈때 사용하는
/// 게임 오브젝트를 리셋하는 클래스
/// 리소스에서 무작위 맵 데이터를 불러와서 게임 오브젝트를 생성하고
/// 이벤트 버스를 통해 맵 데이터를 전달해준다.
/// </summary>
public class EnviromentResetter : MonoBehaviour
{
    private MapTheme? _currentTheme = null;
    
    private string _dataDirectory = "Datasets";

    private void Start()
    {
        MapData _map = EscalateMap();
        EventBus.Publish(new MapChangedEvent(_map));
    }
    
    private MapData LoadMap(MapTheme _theme)
    {
        GameObject[] _mapData         = Resources.LoadAll<GameObject>(_dataDirectory + "/MapData/" + _theme);
        GameObject   _selectedMapData = _mapData[Random.Range(0, _mapData.Length)];
        
        if (_selectedMapData != null)
        {
            GameObject _map = Instantiate(_selectedMapData);
            return _map.GetComponent<MapData>();
        }
        
        Debug.LogError("Map data not found");
        return null;

    }
    
    private MapData EscalateMap()
    {
        switch (_currentTheme)
        {
            case null:
                _currentTheme = MapTheme.Spring;
                break;
            case MapTheme.Spring:
                _currentTheme = MapTheme.Summer;
                break;
            case MapTheme.Summer:
                _currentTheme = MapTheme.Fall;
                break;
            case MapTheme.Fall:
                _currentTheme = MapTheme.Winter;
                break;
            case MapTheme.Winter:
                //TODO: END
                break;
        }

        return LoadMap((MapTheme) _currentTheme);
    }
}

public class MapChangedEvent
{
    public readonly MapData mapData;
    
    public MapChangedEvent(MapData _mapData)
    {
        mapData = _mapData;
    }
}
