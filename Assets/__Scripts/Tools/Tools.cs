using UnityEngine;

public static class Tools
{
    /// <summary>
    /// to를 향하는 방향벡터를 반환
    /// 왼쪽 오른쪽으로만 구별함
    /// </summary>
    public static Vector3 DirectionToX(this Vector3 from, Vector3 to)
    {
        return new Vector3(to.x - from.x, 0, 0).normalized;
    }

    /// <summary>
    /// to를 향하는 방향벡터를 반환
    /// 위 아래로만 구별함
    /// </summary>
    public static Vector3 DirectionToY(this Vector3 from, Vector3 to)
    {
        return new Vector3(0, to.y - from.y, 0).normalized;
    }

    /// <summary>
    /// to를 향하는 방향벡터를 반환
    ///  XY평면에서의 방향벡터
    /// </summary>
    public static Vector3 DirectionToXY(this Vector3 from, Vector3 to)
    {
        return new Vector3(to.x - from.x, to.y - from.y, 0).normalized;
    }
}