using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PierceShot : AttackableSkill
{
    private const float RANGE         = 50f;
    private const float COOLDOWN      = 6f;
    private const float STUN_DURATION = 1f;
    private const float SKILL_DAMAGE  = 3f;

    public override void Initialize()
    {
        SkillType   = SkillTypes.Secondary;
        Cooldown    = COOLDOWN;
        SkillDamage = SKILL_DAMAGE;
    }

    public override void Play()
    {
        if (!CanUse()) return;
        if (!ConsumeResource()) return;
        Revolver.NextReloadTime = Revolver.GetNextReloadTime();

        Transform _cachedTransform = Player.transform;
        Vector3   _position        = _cachedTransform.position;

        LayerMask      _layerMask = LayerMask.GetMask("Enemy", "Floor");
        RaycastHit2D[] _hit       = Physics2D.RaycastAll(_position, _cachedTransform.right * (int)Facing, RANGE, _layerMask);
        Player.StartCoroutine(Revolver.DelayAndSetControllable(Player, .2f));
        foreach (var _hitObject in _hit)
        {
            VFXManager.PlayVFX("BulletPop", _hitObject.point, (int)Player.Controller.PlayerFacing);
            Collider2D _hitCollider = _hitObject.collider;

            if (_hitCollider == null) continue;
            
            //RaycastAll과 다르게 RaycastAll2D는 RaycastHit2D[]의 정렬이 가까운 순서대로 되어있으므로
            //벽에 맞았다면 뒤에있는 물체들은 무시해도 된다.
            if (_hitCollider.CompareTag("Floor")) break;

            if (_hitCollider.CompareTag("Enemy"))
            {
                (int _damage, bool _critical) = GetDamage();
                _hitCollider.GetComponent<EnemyBase>().Attacked(_damage, _critical, STUN_DURATION, Player);
            }
        }

        if (Player.Stats.Resource == 0)
        {
            Revolver.Reload();
            return;
        }

        LastUsedTime = Time.time;
    }
}