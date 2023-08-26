using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum PickupType
{
    Resource,
    Exp,
    Health
}

public enum PickupSize
{
    Large  = 150,
    Medium = 25,
    Small  = 1
}

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    private void Start() => Instance ??= this;

    [SerializeField] private Sprite     exp, health, resource;
    [SerializeField] private GameObject pickupPrefab;

    public void GetPickupObject(PickupType p_pickupType, int p_value, Transform p_transform)
    {
        if (p_value <= 0) return;

        int _large  = (p_value)                         / (int)PickupSize.Large;
        int _medium = (p_value % (int)PickupSize.Large) / (int)PickupSize.Medium;
        int _small  = (p_value % (int)PickupSize.Large) % (int)PickupSize.Medium;

        List<GameObject> _pickups = new List<GameObject>();

        for (int _i = 0; _i < _large; _i++) _pickups.Add(GetPickupObject(p_pickupType,  PickupSize.Large,  p_transform));
        for (int _i = 0; _i < _medium; _i++) _pickups.Add(GetPickupObject(p_pickupType, PickupSize.Medium, p_transform));
        for (int _i = 0; _i < _small; _i++) _pickups.Add(GetPickupObject(p_pickupType,  PickupSize.Small,  p_transform));
    }

    private GameObject GetPickupObject(PickupType p_pickupType, PickupSize p_pickupSize, Transform p_transform)
    {
        GameObject     _pickup               = Instantiate(pickupPrefab, p_transform);
        Pickup         _pickupComponent      = _pickup.GetComponent<Pickup>();
        SpriteRenderer _pickupSpriteRenderer = _pickup.GetComponent<SpriteRenderer>();

        _pickup.transform.localScale *= GetPickupSize(p_pickupSize);
        
        _pickupComponent.Initialize(p_pickupType, p_pickupSize);

        _pickupSpriteRenderer.sprite = p_pickupType switch
        {
            PickupType.Resource => resource,
            PickupType.Exp      => exp,
            PickupType.Health   => health,
            _                   => throw new ArgumentOutOfRangeException(nameof(p_pickupType), p_pickupType, null)
        };

        return _pickup;
    }

    private float GetPickupSize(PickupSize p_pickupSize) => p_pickupSize switch
    {
        PickupSize.Large  => 2f,
        PickupSize.Medium => 1.5f,
        PickupSize.Small  => 1f,
        _                 => 1f
    };
}