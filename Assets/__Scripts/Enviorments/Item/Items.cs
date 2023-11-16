using System;
using System.Collections.Generic;
using UnityEngine;

public class Items
{
    private Player               _owner;
    private Dictionary<int, int> _items;
    private Action               _onItemAdded;

    [Inject] private ItemManager _itemManager;

    public Dictionary<int, int> GetItems() => _items;

    public Items(Player _owner)
    {
        this._owner = _owner;
        _items      = new Dictionary<int, int>();
        DIContainer.Inject(this);
    }

    public void AddItem(int _itemID)
    {
        bool _alreadyHasItem = false;
        if (_items.TryGetValue(_itemID, out _))
        {
            _items[_itemID] += 1;
            _alreadyHasItem =  true;
        }
        else
            _items.Add(_itemID, 1);

        List<StatModifier> _itemStatsMods = _itemManager.GetItem(_itemID).statValues;
        if (_itemStatsMods.Count > 0)
        {
            foreach (StatModifier _itemStatMod in _itemStatsMods)
                _owner.Stats.ApplyStats(_itemStatMod);
        }

        if (!_alreadyHasItem)
        {
            switch (_itemID)
            {
                case 0:
                    _owner._onAttacked += _ => FirstAidKit.Effect(_owner, _items);
                    break;
                case 4:
                    _owner.Controller._onAttackHit += _targetEnemy => StunGranade.Effect(_targetEnemy, _items);
                    break;
            }
        }

        EventBus.Publish(new UpdateItemEvent(this, _itemID, 1));
        _onItemAdded?.Invoke();
    }

    public bool RemoveItem(int _itemID, int _amount)
    {
        if (!_items.TryGetValue(_itemID, out int _itemAmount))
            return false; //아이템이 없음

        if (_itemAmount < _amount)
            return false; //아이템이 부족함

        EventBus.Publish(new UpdateItemEvent(this, _itemID, -_amount));
        _items[_itemID] -= _amount;

        return true;
    }

    public int GetItemAmount(int _itemID)
    {
        return !_items.TryGetValue(_itemID, out int _itemAmount) ? 0 : _itemAmount;
    }
}

public class UpdateItemEvent
{
    public Items Item;
    public int   ItemID;
    public int   ChangeAmount;

    private UpdateItemEvent() { }

    public UpdateItemEvent(Items _item, int _itemID, int _changeAmount)
    {
        Item = _item;
    }

    //TODO : 이렇게 써도 된다면 다른 이벤트들도 이렇게 써도 되는거 아닌가?
    //아니면 이벤트 전용 풀링을 하는것도 좋을듯
    private static readonly UpdateItemEvent StaticEvent = new();

    public static void Publish(Items _items)
    {
        StaticEvent.Item = _items;
        EventBus.Publish(StaticEvent);
    }
}