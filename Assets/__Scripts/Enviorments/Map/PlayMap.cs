using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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

    #if UNITY_EDITOR
    public void Initialize()
    {
        _mapSize           = GetMapSize();
        Transform _cachedTransform = transform;
        _cachedTransform.position = new Vector3(0, _mapSize.y);


        if (_boundCollider == null)
        {
            _boundCollider = gameObject.AddComponent<PolygonCollider2D>();
            SetBoundCollider();
        }
        else
            SetBoundCollider();


        //ShadowCaster2D 생성
        Transform _mapTransform = _cachedTransform.Find("Map");
        if (!_mapTransform.TryGetComponent(out ShadowCaster2DCreator _))
            _mapTransform.AddComponent<ShadowCaster2DCreator>().Create();

        _mapTransform.gameObject.layer = LayerMask.NameToLayer("Floor");

        if (_mapTransform.TryGetComponent(out CompositeCollider2D _compositeCollider))
        {
            _compositeCollider.geometryType   = CompositeCollider2D.GeometryType.Polygons;
            _compositeCollider.offsetDistance = 0;
        }

        var _destroyRequiredObjects = new List<GameObject>();
        foreach (Transform _children in _cachedTransform)
            if (_children.name.Contains("prototype", StringComparison.OrdinalIgnoreCase))
                _destroyRequiredObjects.Add(_children.gameObject);
        
        foreach (GameObject _destroyRequiredObject in _destroyRequiredObjects) 
            DestroyImmediate(_destroyRequiredObject);
        

        Transform _collisions = _cachedTransform.Find("Collision").transform;
        foreach (Transform _transform in _collisions)
        {
            _transform.gameObject.name  = "AdditionalCollision";
            _transform.gameObject.layer = LayerMask.NameToLayer("Floor");

            if (!_transform.TryGetComponent(out ShadowCaster2D _))
            {
                ShadowCaster2D _shadowCaster2D = _transform.AddComponent<ShadowCaster2D>();
                _shadowCaster2D.selfShadows = true;
            }
        }
    }
    #endif

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