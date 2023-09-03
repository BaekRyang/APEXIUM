using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Mathematics;
using UnityEngine;

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

    public static string ConvertDamageUnit(this int p_pDamage, bool p_isCritical, DamageUnitType p_type = DamageUnitType.Full)
    {
        string _damage = "";
        switch (p_type)
        {
            case DamageUnitType.Full:
                _damage = UseKoreanUnit ?
                    p_pDamage switch
                    {
                        >= 100000000 => $"{p_pDamage / 100000000:0}억{p_pDamage / 10000:0}만{p_pDamage % 1000:000}",
                        >= 10000     => $"{p_pDamage / 10000:0}만{p_pDamage     % 10000:000}",
                        _            => p_pDamage.ToString()
                    } :
                    p_pDamage.ToString();
                break;
            case DamageUnitType.Short:
                _damage = UseKoreanUnit ?
                    p_pDamage switch
                    {
                        //백만 단위에는 M 붙이고, 천 단위에는 K 붙인다. (소수 1~2자리까지, 나머지는 표시하지 않음)
                        >= 100000000 => $"{p_pDamage / 100000000f:0.##}억",
                        >= 10000     => $"{p_pDamage / 10000f:0.#}만",
                        _            => p_pDamage.ToString()
                    } :
                    p_pDamage switch
                    {
                        //백만 단위에는 M 붙이고, 천 단위에는 K 붙인다. (소수 1~2자리까지, 나머지는 표시하지 않음)
                        >= 1000000 => $"{p_pDamage / 1000000f:0.##}M",
                        >= 1000    => $"{p_pDamage / 1000f:0.#}K",
                        _          => p_pDamage.ToString()
                    };
                break;
        }

        if (p_isCritical) _damage = "<size=12>" + _damage[0] + "</size>" + _damage.Substring(1); //맨 앞글자 강조

        string _str = (p_isCritical ?
            "<color=#FF5500>" :
            "<color=#FFDDCC>") + _damage + (p_isCritical ?
            "!" :
            "") + "</color>";

        return _str;
    }

    public static IEnumerable<T> GetEnumValues<T>() => Enum.GetValues(typeof(T)).Cast<T>();

    public static float Remap(float p_value, Vector2 p_origin, Vector2 p_target) => 
        p_target.x + (p_value - p_origin.x) * (p_target.y - p_target.x) / (p_origin.y - p_origin.x);

    public static Vector2 Remap(Vector2 p_value, Vector2[] p_origin, Vector2[] p_target)
    {
        return new Vector2(
            Remap(p_value.x, new Vector2(p_origin[0].x, p_origin[1].x), new Vector2(p_target[0].x, p_target[1].x)),
            Remap(p_value.y, new Vector2(p_origin[0].y, p_origin[1].y), new Vector2(p_target[0].y, p_target[1].y)));
    } 
    
    
    public static Vector2 GetResolution(this Sprite p_sprite)
    {
        Debug.Log($"{p_sprite.texture.width}, {p_sprite.texture.height}");
        return new Vector2(p_sprite.texture.width, p_sprite.texture.height);
    }
}