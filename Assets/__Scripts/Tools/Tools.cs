using UnityEngine;

public static class Tools
{
    /// <summary>
    /// Vector3의 x값을 반전
    /// </summary>
    public static void InvertX(ref this Vector3 p_value)
    {
        p_value = new Vector3(-p_value.x, p_value.y, p_value.z);
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
}