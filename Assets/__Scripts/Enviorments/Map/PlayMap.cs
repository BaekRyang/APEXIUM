using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class PlayMap : MonoBehaviour
{
    [SerializeField] private int               _level;
    [SerializeField] private int               _mapIndex;
    [SerializeField] private Vector2           _mapSize;
    [SerializeField] private PolygonCollider2D _boundCollider;

    public int               GetLevel => _level;
    public int               GetIndex => _mapIndex;
    public string            GetName  => $"{_level}-{_mapIndex}";
    public Vector2           GetSize  => _mapSize;
    public PolygonCollider2D GetBound => _boundCollider;

    public Vector2 GetMapSize()
    {
        var _size = GetComponentInChildren<Tilemap>().localBounds.size;
        return new Vector2(_size.x, _size.y);
    }

    public void Initialize()
    {
        _mapSize           = GetMapSize();
        transform.position = new Vector3(0, _mapSize.y);


        if (_boundCollider == null)
        {
            _boundCollider = gameObject.AddComponent<PolygonCollider2D>();
            SetBoundCollider();
        }
        else
            SetBoundCollider();
    }

    private void SetBoundCollider()
    {
        _boundCollider.isTrigger = true;
        _boundCollider.SetPath(0, new[]
                                  {
                                      new Vector2(0,          -_mapSize.y),
                                      new Vector2(_mapSize.x, -_mapSize.y),
                                      new Vector2(_mapSize.x, 0),
                                      new Vector2(0,          0)
                                  });
    }

    public void GetRelativePosition(Player p_player)
    {
        var _playerPosition = p_player.transform.position;
        var _mapPosition    = transform.position;
    }
}