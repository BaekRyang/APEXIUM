using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RevolverShot : Weapon
{
    private float _range = 20f;
    
    public override void Play(int p_damageMultiplier)
    {
        Transform _cachedTransform = transform;
        Vector3   _position        = _cachedTransform.position;

        RaycastHit2D _hit = Physics2D.Raycast(_position, _cachedTransform.right * Facing, _range);

        Collider2D _hitCollider = _hit.collider;

        if (_hitCollider.IsUnityNull()) return;

        if (_hitCollider.CompareTag("Enemy"))
        {
            int _damage = (int)(Damage * p_damageMultiplier);
            _hitCollider.GetComponent<EnemyBase>().Attacked(_damage, player);
        }
    }
}