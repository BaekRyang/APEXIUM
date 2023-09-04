using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spree : AttackableSkill
{
    private const float RANGE               = 30f;
    private const float COOLDOWN            = 12f;
    private const float STUN_DURATION       = 1f;
    private const float SKILL_DAMAGE        = 2.5f;
    private const int   BULLET_SPREAD_ANGLE = 1;
    private const float SPREE_SPEED         = 2f;

    public override void Initialize()
    {
        SkillType   = SkillTypes.Ultimate;
        Cooldown    = COOLDOWN;
        SkillDamage = SKILL_DAMAGE;
    }

    public override bool Play()
    {
        if (!CanUse()) return false;
        if (!ConsumeResource()) return false;

        Player.Controller.SetControllable(false);
        Player.Controller.AddLandingAction(() => Player.Controller.Rigidbody2D.velocity = Vector2.zero);
        Player.StartCoroutine(SpreeBullet());
        LastUsedTime = Time.time;
        return true;
    }

    private IEnumerator SpreeBullet()
    {
        var _attackID = Tools.GetAttackID();
        do
        {
            Revolver.NextReloadTime = Revolver.GetNextReloadTime();

            Transform _cachedTransform = Player.transform;
            Vector3   _position        = _cachedTransform.position;

            //Quaternion은 위 아래로 랜덤이어야 하므로 z축을 기준으로 회전한다.
            Vector3 _randDirection = Quaternion.Euler(0, 0, Random.Range(-BULLET_SPREAD_ANGLE, BULLET_SPREAD_ANGLE)) * (_cachedTransform.right * (int)Facing);

            LayerMask    _layerMask = LayerMask.GetMask("Enemy", "Floor");
            RaycastHit2D _hit       = Physics2D.BoxCast(_position, Vector2.one * .3f, 0, _randDirection, RANGE, _layerMask);

            Collider2D _hitCollider = _hit.collider;

            if (_hitCollider != null)
            {
                VFXManager.PlayVFX("BulletPop", _hit.point, (int)Player.Controller.PlayerFacing);
                if (_hitCollider.CompareTag("Enemy"))
                {
                    (int _damage, bool _critical) = GetDamage();
                    _hitCollider.GetComponent<EnemyBase>().Attacked(_damage, _critical, STUN_DURATION, Player, _attackID);
                }
            }

            yield return new WaitForSeconds(RevolverShot.COOLDOWN / SPREE_SPEED / Stats.AttackSpeed);
        } while (ConsumeResource());

        Revolver.Reload();
        Player.Controller.SetControllable(true);
    }
}