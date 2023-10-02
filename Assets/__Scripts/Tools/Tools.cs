using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public static class Tools
{
    /// <summary>
    /// Vector3의 x값을 반전
    /// </summary>
    public static void InvertX(ref this Vector3 p_value)
    {
        p_value = new(-p_value.x, p_value.y, p_value.z);
    }

    /// <summary>
    /// to를 향하는 방향벡터를 반환
    /// 왼쪽 오른쪽으로만 구별함
    /// </summary>
    public static Vector3 DirectionToX(this Vector3 from, Vector3 to)
    {
        return new Vector3((to - from).x, 0, 0).normalized;
    }

    /// <summary>
    /// to를 향하는 방향벡터를 반환
    /// 위 아래로만 구별함
    /// </summary>
    public static Vector3 DirectionToY(this Vector3 from, Vector3 to)
    {
        return new Vector3(0, (to - from).y, 0).normalized;
    }

    /// <summary>
    /// to를 향하는 방향벡터를 반환
    ///  XY평면에서의 방향벡터
    /// </summary>
    public static Vector3 DirectionToXY(this Vector3 from, Vector3 to)
    {
        return new Vector3((to - from).x, (to - from).y, 0).normalized;
    }

    private static uint ID;

    public static uint GetAttackID()
    {
        //최대값 도달시 0으로 초기화
        if (ID == uint.MaxValue) ID = 0;

        return ID++;
    }

    //데미지를 한글단위로 사용?
    private static bool UseKoreanUnit = true;

    public enum DamageUnitType
    {
        Full,
        Short
    }

    public static string ConvertDamageUnit(this int _pDamage, bool _isCritical, DamageUnitType _type = DamageUnitType.Full)
    {
        string _damage = "";
        switch (_type)
        {
            case DamageUnitType.Full:
                _damage = UseKoreanUnit ?
                    _pDamage switch
                    {
                        >= 100000000 => $"{_pDamage / 100000000:0}억{_pDamage / 10000:0}만{_pDamage % 1000:000}",
                        >= 10000     => $"{_pDamage / 10000:0}만{_pDamage     % 10000:000}",
                        _            => _pDamage.ToString()
                    } :
                    _pDamage.ToString();
                break;
            case DamageUnitType.Short:
                _damage = UseKoreanUnit ?
                    _pDamage switch
                    {
                        //백만 단위에는 M 붙이고, 천 단위에는 K 붙인다. (소수 1~2자리까지, 나머지는 표시하지 않음)
                        >= 100000000 => $"{_pDamage / 100000000f:0.##}억",
                        >= 10000     => $"{_pDamage / 10000f:0.#}만",
                        _            => _pDamage.ToString()
                    } :
                    _pDamage switch
                    {
                        //백만 단위에는 M 붙이고, 천 단위에는 K 붙인다. (소수 1~2자리까지, 나머지는 표시하지 않음)
                        >= 1000000 => $"{_pDamage / 1000000f:0.##}M",
                        >= 1000    => $"{_pDamage / 1000f:0.#}K",
                        _          => _pDamage.ToString()
                    };
                break;
        }

        if (_isCritical) _damage = "<size=12>" + _damage[0] + "</size>" + _damage.Substring(1); //맨 앞글자 강조

        string _str = (_isCritical ?
            "<color=#FF5500>" :
            "<color=#FFDDCC>") + _damage + (_isCritical ?
            "!" :
            "") + "</color>";

        return _str;
    }

    public static IEnumerable<T> GetEnumValues<T>() => Enum.GetValues(typeof(T)).Cast<T>();

    public static IEnumerable<T> GetEnumValues2<T>()
    {
        foreach (var v in Enum.GetValues(typeof(T)))
        {
            yield return (T)v;
        }
    }

    public static float Remap(float _value, Vector2 _origin, Vector2 _target) =>
        _target.x + (_value - _origin.x) * (_target.y - _target.x) / (_origin.y - _origin.x);

    public static Vector2 Remap(Vector2 _value, Vector2[] _origin, Vector2[] _target)
    {
        return new Vector2(
            Remap(_value.x, new Vector2(_origin[0].x, _origin[1].x), new Vector2(_target[0].x, _target[1].x)),
            Remap(_value.y, new Vector2(_origin[0].y, _origin[1].y), new Vector2(_target[0].y, _target[1].y)));
    }

    public static Vector2 Remap(Vector3 _value, Vector2[] _origin, Vector2[] _target)
    {
        return new Vector2(
            Remap(_value.x, new Vector2(_origin[0].x, _origin[1].x), new Vector2(_target[0].x, _target[1].x)),
            Remap(_value.y, new Vector2(_origin[0].y, _origin[1].y), new Vector2(_target[0].y, _target[1].y)));
    }
    
    public static Vector2 GetResolution(this Sprite _sprite)
    {
        Debug.Log($"{_sprite.texture.width}, {_sprite.texture.height}");
        return new Vector2(_sprite.texture.width, _sprite.texture.height);
    }

    public enum KeyType
    {
        Keyboard,
        Mouse,
        Gamepad
    }

    public static void RebindKeymap(this PlayerInput _input, string _actionName, KeyType _schema, string _targetKey)
    {
        _input.enabled = false;
        string _targetSchema = _schema switch
        {
            KeyType.Keyboard => "<Keyboard>/",
            KeyType.Mouse    => "<Mouse>/",
            KeyType.Gamepad  => "<Gamepad>/",
        };
        
        int _control = _schema switch
        {
            KeyType.Keyboard => 0,
            KeyType.Gamepad  => 1,
        };
        
        InputAction _action    = _input.actions.FindAction(_actionName);
        int _bindIndex = _action.GetBindingIndexForControl(_action.controls[_control]);
        
        _action.ApplyBindingOverride(_bindIndex, _targetSchema + _targetKey);
        _input.enabled = true;
    }
}