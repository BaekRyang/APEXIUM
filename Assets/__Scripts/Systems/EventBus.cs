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

    private static Dictionary<int, List<Delegate>> TempTable = new();
    private static int                             currentPublishDepth;

    public static void Publish<T>(params T[] _data)
    {
        //TempTable.Add(currentDepth, EventTable[typeof(T)]);

        var _type = typeof(T);

        if (!EventTable.ContainsKey(_type))
        {
            Debug.Log($"<color=blue>EventBus</color> - Publish : No Subscribers");
            return;
        }

        if (TempTable.ContainsKey(currentPublishDepth) == false)
            TempTable.Add(currentPublishDepth, new List<Delegate>(100));

        var _tempList = TempTable[currentPublishDepth];
        _tempList.Clear();
        _tempList.AddRange(EventTable[typeof(T)]);

        currentPublishDepth++;

        foreach (var _action in _tempList)
        {
            foreach (T _x in _data)
            {
                Debug.Log($"<color=blue>EventBus</color> - Publish : {_x.GetType()}");
                (_action as Action<T>)!(_x);
            }
        }

        currentPublishDepth--;
    }

    public static Action Subscribe<T>(Action<T> _action)
    {
        var _type = typeof(T);
        if (EventTable.ContainsKey(_type) == false)
        {
            EventTable.Add(_type, new List<Delegate>());
            Debug.Log($"<color=blue>EventBus</color> - {_type.Name} has been <b>registered</b>");
        }

        EventTable[_type].Add(_action);
        Debug.Log($"<color=blue>EventBus</color> - {_type.Name} has been <b>subscribed</b>");

        return () => { Unsubscribe(_action); };
    }

    public static void Unsubscribe<T>(System.Action<T> _action)
    {
        var _type = typeof(T);
        if (EventTable.ContainsKey(_type) == false)
        {
            return;
        }

        Debug.Log($"<color=blue>EventBus</color> - {_type.Name} has been <b>unsubscribed</b>");

        EventTable[_type].Remove(_action);
    }
}