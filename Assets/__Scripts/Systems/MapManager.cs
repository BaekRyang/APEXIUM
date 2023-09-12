using System.Collections.Generic;
using UnityEngine;

public enum MapType
{
    Normal,
    Boss
}

public class MapManager : MonoBehaviour
{
    private Dictionary<MapType, PlayMap> maps;

    public PlayMap GetMap(MapType _mapType)
    {
        return maps.TryGetValue(_mapType, out PlayMap _map) ?
            _map :
            null;
    }

    public void AssignMap(Transform _mapsTransform)
    {
        maps = new Dictionary<MapType, PlayMap>();
        foreach (Transform _child in _mapsTransform)
        {
            if (TryGetComponent(out BossRoom _bossRoom))
                maps.Add(MapType.Boss, _bossRoom);
            else if (TryGetComponent(out PlayMap _playMap))
                maps.Add(MapType.Normal, _playMap);
        }
    }
}