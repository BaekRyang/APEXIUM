using System;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType
{
    Item,
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
    [SerializeField] private ItemData              itemList;
    private readonly         Dictionary<int, Item> _items = new();
    
    public Item GetItem(int _itemID)
    {
        if (_items.TryGetValue(_itemID, out Item _item)) return _item;
        
        Debug.Log($"<color=red>ItemManager</color> : {_itemID} is not exist!");
        return null;
    }

    private void OnEnable()
    {
        EventBus.Subscribe<ItemSpawnEvent>(InstantiateItemHandler);

        MakeItems();
    }

    private void MakeItems()
    {
        foreach (Item _itemListItem in itemList.items) 
            _items.Add(_itemListItem.id, _itemListItem);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ItemSpawnEvent>(InstantiateItemHandler);
    }

    private void InstantiateItemHandler(ItemSpawnEvent _event)
    {
        InstantiatePickupObjects(_event.pickupType, _event.value, _event.position);
    }

    private void InstantiatePickupObjects(PickupType _pickupType, int _value, Vector3 _position)
    {
        if (_value < 0) return;

        if (_pickupType == PickupType.Item) //아이템일때
        {
            Pickup _itemPickup = PickupPool.Instance.GetAvailablePickupComponents(_pickupType, _value)[0];
            PickupInitialize(_itemPickup, _value, _position).Activate();
            return;
        }

        //아이템이 아닐 때
        int _large  = (_value)                         / (int)PickupSize.Large;
        int _medium = (_value % (int)PickupSize.Large) / (int)PickupSize.Medium;
        int _small  = (_value % (int)PickupSize.Large) % (int)PickupSize.Medium;

        var _valueTuples = new[]
                           {
                               (PickupSize.Large, _large),
                               (PickupSize.Medium, _medium),
                               (PickupSize.Small, _small)
                           };

        foreach ((PickupSize _pickupSize, int _pickupValue) in _valueTuples)
        {
            foreach (Pickup _pickup in PickupPool.Instance.GetAvailablePickupComponents(_pickupType, _pickupValue))
                PickupInitialize(_pickup, _pickupSize, _position).Activate();
        }
    }

    private Pickup PickupInitialize(Pickup _pickup, PickupSize _size, Vector3 _position)
    {
        _pickup.PickupValue          = (int)_size;
        _pickup.transform.position   = _position + Vector3.up;
        _pickup.transform.localScale = Vector3.one * GetPickupSize(_size);
        _pickup.gameObject.SetActive(true);
        return _pickup;
    }

    private Pickup PickupInitialize(Pickup _pickup, int _size, Vector3 _position)
    {
        _pickup.PickupValue          = _size;
        _pickup.transform.position   = _position + Vector3.up;
        _pickup.gameObject.SetActive(true);
        return _pickup;
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