using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class PickupPool : MonoBehaviour
{
    public static PickupPool Instance;

    private readonly Dictionary<PickupType, List<Pickup>> PickupPools          = new();
    private readonly Dictionary<PickupType, Transform>    PickupPoolTransforms = new();

    [SerializeField] private Sprite                  _nullSprite;
    private                  Dictionary<int, Sprite> _itemSprites = new();

    [Inject] private ItemManager _itemManager;

    private void Start()
    {
        Instance ??= this;

        foreach (PickupType _value in Tools.GetEnumValues<PickupType>())
        {
            PickupPools.Add(_value, new List<Pickup>());
            PickupPoolTransforms.Add(_value, new GameObject(_value.ToString()).transform);
        }

        //위치 조정
        foreach ((_, Transform _transform) in PickupPoolTransforms)
        {
            _transform.position = Vector3.zero;
            _transform.parent   = transform;
        }

        DIContainer.Inject(this);
    }

    [SerializeField] private Sprite[]   exp, health, resource;
    [SerializeField] private GameObject pickupPrefab;

    public List<Pickup> GetAvailablePickupComponents(PickupType _pickupType, int _value)
    {
        //TODO:Lock이 필요하지 않을까?
        int _availablePickupCount = GetAvailablePickupCount(_pickupType);
        Debug.Log($"<color=green>Available {_pickupType} : {_availablePickupCount}</color>");

        if (_pickupType == PickupType.Item) //아이템 전용
        {
            if (_availablePickupCount <= 0)
                InstantiatePickupObject(_pickupType, _value);
            
            return PickupPools[_pickupType]
                  .Select(_pickup => _pickup)                       //Pool안에 Pickup Component를
                  .Where(_pickup => !_pickup.gameObject.activeSelf) //사용 가능한 것 만
                  .Take(1)
                  .ToList();
        }

        if (_availablePickupCount < _value) //풀에 픽업이 부족하면
        {
            Debug.Log($"<color=green>Create {_pickupType} : {_value - _availablePickupCount}</color>");
            for (int _i = 0; _i < _value - _availablePickupCount; _i++) //모자란 만큼 만든다.
                InstantiatePickupObject(_pickupType);
        }

        return PickupPools[_pickupType]
              .Select(_pickup => _pickup)                       //Pool안에 Pickup Component를
              .Where(_pickup => !_pickup.gameObject.activeSelf) //사용 가능한 것 만
              .Take(_value)                                     //p_count만큼
              .ToList();                                        //List로 반환
    }

    private int GetAvailablePickupCount(PickupType _pickupType)
    {
        return PickupPools[_pickupType]
              .Select(_pickup => _pickup)
              .Count(_pickup => !_pickup.gameObject.activeSelf);
    }

    private void InstantiatePickupObject(PickupType _pickupType, int _id = -1)
    {
        Debug.Log($"<color=green>Instantiate {_pickupType} / id : {_id}</color>");
        GameObject     _pickup               = Instantiate(pickupPrefab, PickupPoolTransforms[_pickupType]);
        SpriteRenderer _pickupSpriteRenderer = _pickup.GetComponent<SpriteRenderer>();

        Pickup _pickupComponent = _pickup.GetComponent<Pickup>();

        _pickupSpriteRenderer.sprite = _pickupType switch
        {
            PickupType.Item     => _itemManager.GetItem(_id).sprite,
            PickupType.Resource => resource[Random.Range(0, resource.Length)],
            PickupType.Exp      => exp[Random.Range(0,      exp.Length)],
            PickupType.Health   => health[Random.Range(0,   health.Length)],
            _                   => throw new ArgumentOutOfRangeException(nameof(_pickupType), _pickupType, null)
        };

        InitializePickupObject(_pickupType, _pickup, _pickupComponent, _pickupSpriteRenderer);

        _pickup.SetActive(false);
        PickupPools[_pickupType].Add(_pickupComponent);
    }

    private static void InitializePickupObject(PickupType _pickupType, GameObject _pickup, Pickup _pickupComponent, SpriteRenderer _pickupSpriteRenderer)
    {
        _pickupComponent.pickupType = _pickupType;
        switch (_pickupType)
        {
            case PickupType.Item:
            {
                if (_pickup.TryGetComponent(out Light2D _light2D))
                    Destroy(_light2D);

                break;
            }
            case PickupType.Resource:
            {
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
            }
            case PickupType.Exp:
            {
                _pickupSpriteRenderer.color = new Color(.5f, 0.9212599f, 1, 1);
                break;
            }
            case PickupType.Health:
            {
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(_pickupType), _pickupType, null);
        }
    }

    private Sprite GetSpriteFromID(int _id)
    {
        return _nullSprite;
    }
}