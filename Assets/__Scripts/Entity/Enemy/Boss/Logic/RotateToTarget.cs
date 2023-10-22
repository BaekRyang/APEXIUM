using System;
using UnityEngine;

public class RotateToTarget : MonoBehaviour
{
    [SerializeField] private Transform target;

    private void FixedUpdate()
    {
        Vector3 targetDir = target.position - transform.position;
        
        float deg = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(deg + 90, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 20f);
        Debug.DrawLine(transform.position, target.position, Color.red);
    
    }
}
