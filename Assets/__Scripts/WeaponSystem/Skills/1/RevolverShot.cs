using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;

public class RevolverShot : Weapon
{
    private const float RANGE             = 20f;
    private const float COOLDOWN          = .25f;
    private const float STUN_DURATION     = 0f;
    private const float DAMAGE_MULTIPLIER = 1f;

    public void OnEnable()
    {
        cooldown ??= COOLDOWN;
        skillDamage = DAMAGE_MULTIPLIER;
    }

    public override bool Play()
    {
        if (!base.Play()) return false; //쿨타임 체크

        Transform _cachedTransform = transform;
        Vector3   _position        = _cachedTransform.position;

        RaycastHit2D _hit = Physics2D.Raycast(_position, _cachedTransform.right * (int)Facing, RANGE);

        Collider2D _hitCollider = _hit.collider;

        if (_hitCollider.IsUnityNull()) return false;

        if (_hitCollider.CompareTag("Enemy"))
        {
            int _damage = GetDamage();
            _hitCollider.GetComponent<EnemyBase>().Attacked(_damage, STUN_DURATION, player);
        }

        return true;
    }
}