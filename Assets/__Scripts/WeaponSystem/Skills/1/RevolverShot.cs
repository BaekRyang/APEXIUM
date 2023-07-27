using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;

public class RevolverShot : Weapon
{
    private float _range = 20f;
    private const float COOLDOWN = .025f;
    private const float STUN_DURATION = 0f;

    public void OnEnable()
    {
        cooldown ??= COOLDOWN;
    }

    public override bool Play(int p_damageMultiplier)
    {
        if (base.Play(p_damageMultiplier) == false) return false; //쿨타임 체크
        
        Transform _cachedTransform = transform;
        Vector3   _position        = _cachedTransform.position;

        RaycastHit2D _hit = Physics2D.Raycast(_position, _cachedTransform.right * (int)Facing, _range);

        Collider2D _hitCollider = _hit.collider;

        if (_hitCollider.IsUnityNull()) return false;

        if (_hitCollider.CompareTag("Enemy"))
        {
            //0.8에서 1.2 사이의 값 생성 소수 둘째자리까지
            float _randomDamageMultiplier = UnityEngine.Random.Range(0.8f, 1.2f);
            int   _damage                 = (int)(Damage * p_damageMultiplier * _randomDamageMultiplier);
            _hitCollider.GetComponent<EnemyBase>().Attacked(_damage, STUN_DURATION, player);
        }

        return true;
    }
}