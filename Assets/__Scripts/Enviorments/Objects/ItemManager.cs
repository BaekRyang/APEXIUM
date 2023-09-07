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

    public void InstantiatePickupObjects(PickupType _pickupType, int _value, Transform _transform)
    {
        if (_value <= 0) return;

        int _large  = (_value)                         / (int)PickupSize.Large;
        int _medium = (_value % (int)PickupSize.Large) / (int)PickupSize.Medium;
        int _small  = (_value % (int)PickupSize.Large) % (int)PickupSize.Medium;


        foreach ((PickupSize _pickupSize, int _pickupValue) in new[] { (PickupSize.Large, _large), (PickupSize.Medium, _medium), (PickupSize.Small, _small) })
            foreach (Pickup _pickup in PickupPool.Instance.GetAvailablePickupComponents(_pickupType, _pickupValue))
                PickupInitialize(_pickup, _pickupSize).Activate();

        return;
    
        Pickup PickupInitialize(Pickup _pickup, PickupSize _size)
        {
            _pickup.PickupValue          = (int)_size;
            _pickup.transform.position   = _transform.position + Vector3.up;
            _pickup.transform.localScale = Vector3.one * GetPickupSize(_size);
            _pickup.gameObject.SetActive(true);
            return _pickup;
        }
    }

    private static float GetPickupSize(PickupSize _pickupSize) => _pickupSize switch
    {
        PickupSize.Large  => 2f,
        PickupSize.Medium => 1.5f,
        PickupSize.Small  => 1f,
        _                 => 1f
    };
}