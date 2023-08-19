using UnityEngine;

public static class Tools
{
    public static void InvertX(this Vector3 p_value)
    {
        Debug.Log("invertX");
        p_value = new Vector3(-p_value.x, p_value.y, p_value.z);
    }
}
