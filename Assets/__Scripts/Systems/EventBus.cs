using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus
{
    private static Dictionary<Type, List<Delegate> > EventTable = new();
    
    public static void Publish<T>(T _data)
    {
        var _type = typeof(T);
        if (EventTable.ContainsKey(_type) == false)
        {
            Debug.Log($"<color=blue>EventBus</color> - Publish : No Subscribers");
            return;
        }

        Debug.Log($"<color=blue>EventBus</color> - Publish to {EventTable[_type].Count} subscribers");
        foreach (var _action in EventTable[_type])
        {
            (_action as Action<T>)!(_data);
        }
    }
    
    public static void Subscribe<T>(Action<T> _action)
    {
        var _type = typeof(T);
        if (EventTable.ContainsKey(_type) == false)
        {
            EventTable.Add(_type, new   List<Delegate>());
            Debug.Log($"<color=blue>EventBus</color> - {_type.Name} has been <b>registered</b>");
        }
        EventTable[_type].Add(_action);
        Debug.Log($"<color=blue>EventBus</color> - {_type.Name} has been subscribed");
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