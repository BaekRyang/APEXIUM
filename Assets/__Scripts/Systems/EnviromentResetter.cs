using System;
using UnityEngine;
using Random = UnityEngine.Random;

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
