using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PierceShot : AttackableSkill
{
    private const float RANGE             = 20f;
    private const float COOLDOWN          = 6f;
    private const float STUN_DURATION     = 1f;
    private const float DAMAGE_MULTIPLIER = 3f;

    public void OnEnable()
    {
        skillType   = SkillTypes.Secondary;
        cooldown    = COOLDOWN;
        skillDamage = DAMAGE_MULTIPLIER;
    }

    public override bool Play()
    {
        if (!base.Play()) return false; //쿨타임 체크

        Transform _cachedTransform = transform;
        Vector3   _position        = _cachedTransform.position;

        RaycastHit2D[] _hit = Physics2D.RaycastAll(_position, _cachedTransform.right * (int)Facing, RANGE);

        foreach (var _hitObject in _hit)
        {
            Collider2D _hitCollider = _hitObject.collider;

            if (_hitCollider.IsUnityNull()) return false;

            if (_hitCollider.CompareTag("Enemy"))
            {
                int _damage = GetDamage();
                _hitCollider.GetComponent<EnemyBase>().Attacked(_damage, STUN_DURATION, player);
            }
        }

        return true;
    }
}