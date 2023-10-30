using System;
using System.Collections.Generic;
using UnityEngine;

public class SubscribeObject
{
    private List<Delegate> _unsubscribeActions = new();

    public void Subscribe<T>(System.Action<T> _action)
    {
        _unsubscribeActions.Add(EventBus.Subscribe(_action));
    }

    public void UnsubscribeAll()
    {
        foreach (Action _unsubscribeAction in _unsubscribeActions) 
            _unsubscribeAction.Invoke();
        
        _unsubscribeActions.Clear();
    }
}