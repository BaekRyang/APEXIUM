using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

    private static void CustomShader(Vector4 Source, Vector4 Target, float Range)
    {
        Vector4 returnColor;
        
        //Source의 RGB값중 Target의 값과 Range만큼 범위 안으로 비슷한 값을 찾는다.
        //그 값을 returnColor에 저장한다.
        
        if (Source.x >= Target.x - Range && Source.x <= Target.x + Range)
            returnColor.x = Source.x;
        
        if (Source.y >= Target.y - Range && Source.y <= Target.y + Range)
            returnColor.y = Source.y;

        if (Source.z >= Target.z - Range && Source.z <= Target.z + Range)
            returnColor.z = Source.z;
    }
}