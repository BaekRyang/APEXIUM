using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
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
        //모든 타일맵의 localBounds.size 중 가장 큰 값을 저장
        Vector3 _size = Vector3.zero;
        foreach (Tilemap _child in GetComponentsInChildren<Tilemap>())
            _size = Vector3.Max(_size, _child.localBounds.size);

        // Vector3 _linqSize = GetComponentsInChildren<Tilemap>().
        //     Aggregate(Vector3.zero ,(_previous, _current) => Vector3.Max(_previous, _current.localBounds.size));
        //동일한 기능을 한다.
        return _size;
    }

    public Vector2 GetMapCenterPosition() =>
        (Vector2)transform.position + new Vector2(_mapSize.x / 2, -_mapSize.y / 2);

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


        //ShadowCaster2D 생성
        Transform _mapTransform = transform.Find("Map");
        if (!_mapTransform.TryGetComponent(out ShadowCaster2DCreator _))
            _mapTransform.AddComponent<ShadowCaster2DCreator>().Create();
        
        _mapTransform.gameObject.layer = LayerMask.NameToLayer("Floor");

        foreach (Transform _children in transform)
            if (_children.name.Contains("prototype", StringComparison.OrdinalIgnoreCase))
                DestroyImmediate(_children.gameObject);
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
}