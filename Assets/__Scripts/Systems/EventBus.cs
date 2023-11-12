using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus
{
    private static readonly Dictionary<Type, List<Delegate>> EventTable = new();

    //이벤트 재귀호출시에 사용하는 임시 테이블
    //해당 깊이의 이벤트 리스트를 갖는다.
    //이벤트 호출시 해당 이벤트에서 테이블을 수정하는 경우가 발생할 수 있기 떄문에
    //이벤트 호출시에는 임시 테이블에 복사해서 사용한다.
    private static readonly Dictionary<int, List<Delegate>> TempTable = new();

    //이벤트 재귀호출시에 사용하는 깊이값
    private static int CurrentPublishDepth;

    public static void Publish<T>(params T[] _data)
    {
        Type _type = typeof(T);

        if (!EventTable.ContainsKey(_type))
        {
            Debug.Log($"<color=blue>EventBus</color> - Publish : No Subscribers");
            return;
        }

        if (!TempTable.ContainsKey(CurrentPublishDepth))
            TempTable.Add(CurrentPublishDepth, new List<Delegate>(100));

        List<Delegate> _tempList = TempTable[CurrentPublishDepth];
        _tempList.Clear();
        _tempList.AddRange(EventTable[typeof(T)]);

        //TODO : 이 방식이 더 좋은거 같은데
        // List<Delegate> _fixedTempList = new List<Delegate>(EventTable[typeof(T)]);

        CurrentPublishDepth++;

        //여기서 이벤트 테이블 수정이 발생한다면 오류가 발생하기 때문에
        //임시 테이블을 사용한다.
        foreach (Delegate _action in _tempList)
        {
            foreach (T _x in _data)
            {
                Debug.Log($"<color=blue>EventBus</color> - Publish : {_x.GetType()}");
                (_action as Action<T>)!(_x);
            }
        }

        CurrentPublishDepth--;
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