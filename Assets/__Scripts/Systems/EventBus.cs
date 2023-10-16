using System;
using System.Collections.Generic;
using UnityEngine;

public static class StaticVar<T> where T : new()
{
    private static T _value = new();
    public static  T GetValue() => _value;
}

public static class EventBus
{
    private static Dictionary<Type, List<Delegate>> EventTable = new();

    public static void PublishOne<T>(Action<T> _settingFunc) where T : new()
    {
        var a = StaticVar<T>.GetValue();
        _settingFunc(a);
        Publish(a);
    }

    public static void Publish<T>(params T[] _data)
    {
        var _type = typeof(T);

        if (EventTable.ContainsKey(_type) == false)
        {
            Debug.Log($"<color=blue>EventBus</color> - Publish : No Subscribers");
            return;
        }

        Debug.Log($"<color=blue>EventBus</color> - Publish to {EventTable[_type].Count} subscribers");
        foreach (var _action in EventTable[_type])
            foreach (T _x in _data)
            {
                Debug.Log($"<color=blue>EventBus</color> - Publish : {_x.GetType()}");
                (_action as Action<T>)!(_x);
            }
    }

    public static void Subscribe<T>(Action<T> _action)
    {
        var _type = typeof(T);
        if (EventTable.ContainsKey(_type) == false)
        {
            EventTable.Add(_type, new List<Delegate>());
            Debug.Log($"<color=blue>EventBus</color> - {_type.Name} has been <b>registered</b>");
        }

        EventTable[_type].Add(_action);
        Debug.Log($"<color=blue>EventBus</color> - {_type.Name} has been <b>subscribed</b>");
    }

    public static void Unsubscribe<T>(System.Action<T> _action)
    {
        var _type = typeof(T);
        if (EventTable.ContainsKey(_type) == false)
        {
            return;
        }

        EventTable[_type].Remove(_action);
    }
}