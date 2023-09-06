using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class PickupPool : MonoBehaviour
{
    public static PickupPool Instance;

    private static Dictionary<PickupType, List<Pickup>> _pickupPools = new()
                                                                       {
                                                                           { PickupType.Exp, new List<Pickup>() },
                                                                           { PickupType.Health, new List<Pickup>() },
                                                                           { PickupType.Resource, new List<Pickup>() }
                                                                       };

    private static Dictionary<PickupType, Transform> _pickupPoolTransforms = new();

    private void Start()
    {
        Instance ??= this;

        //PickupTypes의 각 오브젝트를 Pool로 만들기 위해 저장한다.
        // foreach (var _type in Enum.GetValues(typeof(PickupType)).Cast<PickupType>())
        //     _pickupPoolTransforms.Add(_type, new GameObject(_type.ToString()).transform);

        foreach (var _convertEnumValue in Tools.GetEnumValues2<PickupType>())
            _pickupPoolTransforms.Add(_convertEnumValue, new GameObject(_convertEnumValue.ToString()).transform);

        //위치 조정
        foreach ((_, Transform _transform) in _pickupPoolTransforms)
        {
            _transform.position = Vector3.zero;
            _transform.parent   = transform;
        }
    }

    [SerializeField] private Sprite[]   exp, health, resource;
    [SerializeField] private GameObject pickupPrefab;

    public List<GameObject> GetAvailablePickupObjects(PickupType p_pickupType, int p_count)
    {
        //TODO:Lock이 필요하지 않을까?
        int _availablePickupCount = GetAvailablePickupCount(p_pickupType);

        if (_availablePickupCount < p_count)
            for (int _i = 0; _i < p_count - _availablePickupCount; _i++)
                InstantiatePickupObject(p_pickupType);


        return _pickupPools[p_pickupType]
              .Select(p_pickup => p_pickup.gameObject)            //Pool안에 Pickup GameObject를
              .Where(p_pickup => !p_pickup.gameObject.activeSelf) //사용 가능한 것 만
              .Take(p_count)                                      //p_count만큼
              .ToList();                                          //List로 반환
    }

    public List<Pickup> GetAvailablePickupComponents(PickupType p_pickupType, int p_count)
    {
        //TODO:Lock이 필요하지 않을까?
        int _availablePickupCount = GetAvailablePickupCount(p_pickupType);
        if (_availablePickupCount < p_count)
            for (int _i = 0; _i < p_count - _availablePickupCount; _i++)
                InstantiatePickupObject(p_pickupType);


        return _pickupPools[p_pickupType]
              .Select(p_pickup => p_pickup)                       //Pool안에 Pickup Component를
              .Where(p_pickup => !p_pickup.gameObject.activeSelf) //사용 가능한 것 만
              .Take(p_count)                                      //p_count만큼
              .ToList();                                          //List로 반환
    }

    private int GetAvailablePickupCount(PickupType p_pickupType)
    {
        return _pickupPools[p_pickupType]
              .Select(p_pickup => p_pickup)
              .Count(p_pickup => !p_pickup.gameObject.activeSelf);
    }

    private void InstantiatePickupObject(PickupType p_pickupType)
    {
        GameObject     _pickup               = Instantiate(pickupPrefab, _pickupPoolTransforms[p_pickupType]);
        SpriteRenderer _pickupSpriteRenderer = _pickup.GetComponent<SpriteRenderer>();

        Pickup _pickupComponent = _pickup.GetComponent<Pickup>();
        _pickupComponent.PickupType = p_pickupType;

        _pickupSpriteRenderer.sprite = p_pickupType switch
        {
            //해당 배열의 랜덤값을 가져온다.
            PickupType.Resource => resource[Random.Range(0, resource.Length)],
            PickupType.Exp      => exp[Random.Range(0,      exp.Length)],
            PickupType.Health   => health[Random.Range(0,   health.Length)],
            _                   => throw new ArgumentOutOfRangeException(nameof(p_pickupType), p_pickupType, null)
        };

        switch (p_pickupType)
        {
            case PickupType.Resource:
                Rigidbody2D _rigidbody2D = _pickup.AddComponent<Rigidbody2D>();
                // _rigidbody2D.freezeRotation         = true;
                _rigidbody2D.gravityScale           = 2;
                _rigidbody2D.angularDrag            = Random.Range(1f, 2f);
                _rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

                _rigidbody2D.sharedMaterial = new PhysicsMaterial2D
                                              {
                                                  bounciness = Random.Range(0.4f, .5f),
                                                  friction   = Random.Range(0.2f, .4f)
                                              };


                PolygonCollider2D _collider = _pickup.AddComponent<PolygonCollider2D>();
                _collider.excludeLayers       = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Pickup");
                _pickupComponent._rigidbody2D = _rigidbody2D;
                break;
            case PickupType.Exp:
                _pickupSpriteRenderer.color = new Color(.5f, 0.9212599f, 1, 1);
                break;
            case PickupType.Health:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(p_pickupType), p_pickupType, null);
        }

        _pickup.SetActive(false);
        _pickupPools[p_pickupType].Add(_pickupComponent);
    }
}