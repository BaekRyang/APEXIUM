using System;
using UnityEngine;

public class RotateToTarget : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private bool      pullTargetToThis;
    [SerializeField] private Vector3   pullTargetOffset;

    private void FixedUpdate()
    {
        target.position = pullTargetToThis ? transform.position + pullTargetOffset : target.position;
        
        Vector3 _targetDir = target.position - transform.position;

        float      _degree     = Mathf.Atan2(_targetDir.y, _targetDir.x) * Mathf.Rad2Deg;
        Quaternion _quaternion = Quaternion.AngleAxis(_degree - 90, Vector3.forward);

        transform.rotation = Quaternion.Slerp(transform.rotation, _quaternion, Time.deltaTime * 20f);
        Debug.DrawLine(transform.position, target.position, Color.red);
    }
}