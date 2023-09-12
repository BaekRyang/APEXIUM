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
    
    private void Awake()
    {
        EventBus.Subscribe<ItemSpawnEvent>(InstantiateItemHandler);
    }
    
    private void InstantiateItemHandler(ItemSpawnEvent _event)
    {
        InstantiatePickupObjects(_event.pickupType, _event.amount, _event.position);
    }

    private void InstantiatePickupObjects(PickupType _pickupType, int _value, Vector3 _position)
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
            _pickup.transform.position   = _position + Vector3.up;
            _pickup.transform.localScale = Vector3.one * GetPickupSize(_size);
            _pickup.gameObject.SetActive(true);
            return _pickup;
        }
    }

    private static float GetPickupSize(PickupSize _pickupSize) => _pickupSize switch
    {
        //TODO : 테이블 방식으로 값을 받아오는게 더 나을수도?
        PickupSize.Large  => 2f,
        PickupSize.Medium => 1.5f,
        PickupSize.Small  => 1f,
        _                 => 1f
    };
}