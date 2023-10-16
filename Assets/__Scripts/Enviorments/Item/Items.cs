using System;
using System.Collections.Generic;
using UnityEngine;

public class Items
{
    private Player               _owner;
    private Dictionary<int, int> _items;
    private Action               _onItemAdded;

    public   Dictionary<int, int> GetItem() => _items;
    [Inject] ItemManager          _itemManager;

    public Items(Player _owner)
    {
        this._owner = _owner;
        _items      = new Dictionary<int, int>();
        DIContainer.Inject(this);
    }

    public void AddItem(int _itemID)
    {
        if (!_items.TryGetValue(_itemID, out _))
            _items.Add(_itemID, 1);
        else
            _items[_itemID] += 1;

        List<StatModifier> _itemStatsMods = _itemManager.GetItem(_itemID).statValues;
        if (_itemStatsMods.Count > 0)
        {
            foreach (StatModifier _itemStatMod in _itemStatsMods)
                _owner.Stats.ApplyStats(_itemStatMod);
        }

        List<Effect> _itemEffects = _itemManager.GetItem(_itemID).effect;
        if (_itemEffects.Count > 0)
        {
            foreach (Effect _itemEffect in _itemEffects)
            {
                //이벤트 적용
            }
        }

        GlobalChangeItemEvent.Publish(this);
        _onItemAdded?.Invoke();
    }

    public bool RemoveItem(int _itemID, int _amount)
    {
        if (!_items.TryGetValue(_itemID, out int _itemAmount))
            return false; //아이템이 없음

        if (_itemAmount < _amount)
            return false; //아이템이 부족함

        GlobalChangeItemEvent.Publish(this);
        _items[_itemID] -= _amount;

        return true;
    }
}

public class GlobalChangeItemEvent
{
    public Items Item;

    private GlobalChangeItemEvent() { }

    private static GlobalChangeItemEvent StaticEvent = new();

    public static void Publish(Items _items)
    {
        StaticEvent.Item = _items;
        EventBus.Publish(StaticEvent);
    }
}