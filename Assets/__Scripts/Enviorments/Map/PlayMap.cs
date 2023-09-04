using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class PlayMap : MonoBehaviour
{
    [SerializeField] private int     _level;
    [SerializeField] private int     _mapIndex;
    [SerializeField] private Vector2 _mapSize;

    public int     GetLevel => _level;
    public int     GetIndex => _mapIndex;
    public string  GetName  => $"{_level}-{_mapIndex}";
    public Vector2 GetSize  => _mapSize;

    public Vector2 GetMapSize()
    {
        var _size = GetComponentInChildren<Tilemap>().localBounds.size;
        return new Vector2(_size.x, _size.y);
    }

    public void Initialize()
    {
        _mapSize           = GetMapSize();
        transform.position = new Vector3(0, _mapSize.y);
    }

    public void GetRelativePosition(Player p_player)
    {
        var _playerPosition = p_player.transform.position;
        var _mapPosition    = transform.position;

    }
}