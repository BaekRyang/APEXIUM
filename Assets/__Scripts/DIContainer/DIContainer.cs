using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class InjectAttribute : Attribute
{
    public readonly string key;

    public InjectAttribute()
    {
    }

    public InjectAttribute(string key)
    {
        this.key = key;
    }
}

public class InjectObj
{
    private bool isInjected = false;

    public void CheckAndInject(object o)
    {
        if (isInjected)
        {
            return;
        }

        isInjected = true;
        DIContainer.Inject(o);
    }
}

public class DIContainer
{
    private const string PREFIX = "<color=cyan>DIContainer</color> :";
    
    public static DIContainer Local = new();
    public static DIContainer Global = new();

    private Dictionary<string, object> _objects = new();

    public void Regist<T>(T _type, string _key = "")
    {
        _objects.Add(GetKey(typeof(T), _key), _type);
    }

    private string GetKey(Type _type, string _key = "")
    {
        return _type.Name + "@" + _key;
    }

    public object GetValue(Type _type, string _key)
    {
        string _diKey = GetKey(_type, _key);
        
        if (_objects.TryGetValue(_diKey, out object _o))
        {
            Debug.Log($"{PREFIX} GetValue - " + _diKey + " " + _o);
            return _o;
        }

        return null;
    }

    public static void Inject(object o)
    {
        Debug.Log($"{PREFIX} {o.GetType().Name} Injected");
        //오브젝트 o에서 Inject어트리뷰트가 있는 필드들에 
        //Local에 등록된 값을 넣어본다.
        //없으면 Global에 등록된 값을 넣는다.
        //없으면 예외처리.

        foreach (FieldInfo _fi in o.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance ))
        {
            InjectAttribute _injectAttr = _fi.GetCustomAttribute<InjectAttribute>();
            if (_injectAttr == null) continue;

            object _value = Local.GetValue(_fi.FieldType, _injectAttr.key) ??
                            Global.GetValue(_fi.FieldType, _injectAttr.key);
            if (_value == null)
                throw new Exception($"{PREFIX} Can't Find Object {_fi.FieldType.Name}");

            _fi.SetValue(o, _value);
        }
    }
}