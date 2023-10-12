using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

[Serializable]
public class PlayMap : MonoBehaviour
{
    [SerializeField] private   int               level;
    [SerializeField] private   int               mapIndex;
    [SerializeField] private   Vector2           mapSize;
    [SerializeField] private   PolygonCollider2D boundCollider;
    [SerializeField] public    Transform         bossRoomEntrance;
    [SerializeField] protected Vector2           entrancePositionOffset;
    public                     Vector2           GetEntranceOffset => entrancePositionOffset;
    public                     int               GetLevel          => level;
    public                     int               GetIndex          => mapIndex;
    public                     string            GetName           => $"{level}-{mapIndex}";
    public                     Vector2           GetSize           => mapSize;
    public                     PolygonCollider2D GetBound          => boundCollider;

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
        (Vector2)transform.position + new Vector2(mapSize.x / 2, -mapSize.y / 2);

#if UNITY_EDITOR
    public void Initialize()
    {
        mapSize = GetMapSize();
        Transform _cachedTransform = transform;
        _cachedTransform.position = new Vector3(0, mapSize.y);


        if (boundCollider == null)
        {
            boundCollider = gameObject.AddComponent<PolygonCollider2D>();
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


        //cacheTransform의 자식중 Collision이라는 이름을 가진 오브젝트를 찾아서 있으면 foreach
        Transform _collisions;
        bool      _found = _collisions = _cachedTransform.Find("Collision");

        if (!_found) return;

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
        boundCollider.isTrigger = true;
        boundCollider.SetPath(0, new[]
                                 {
                                     new Vector2(0,         -mapSize.y),
                                     new Vector2(mapSize.x, -mapSize.y),
                                     new Vector2(mapSize.x, 0),
                                     new Vector2(0,         0)
                                 });
    }

    public Tilemap GetTilemap(string _name) => transform.Find(_name).GetComponent<Tilemap>();
    
    public void SetEntranceOffset()
    {
        BossRoomEntrance _bossRoomEntrance = GetComponentInChildren<BossRoomEntrance>();

        if (_bossRoomEntrance == null) return;
        bossRoomEntrance = _bossRoomEntrance.transform;
        entrancePositionOffset = (Vector2)bossRoomEntrance.position //문의 위치(월드좌표)
                               - (Vector2)transform.position        //맵의 위치(월드좌표)
                               + new Vector2(0, GetSize.y);         //해당 오프셋은 맵의 피벗인 왼쪽 위 기준이므로
        //왼쪽 아래 기준으로 바꿔줘야함 (맵의 높이만큼 더해줌)
    }
}