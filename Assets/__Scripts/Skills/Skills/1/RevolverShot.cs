using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;

public class RevolverShot : AttackableSkill
{
    private const float RANGE               = 40f;
    private const float COOLDOWN            = .25f;
    private const float STUN_DURATION       = 0f;
    private const float DAMAGE_MULTIPLIER   = 1f;
    private const float RELOAD_TIME_SECONDS = 2f;

    //재장전중 총알이 장전되었다면 재장전을 끊기 위하여 총알이 장전되었는지 확인하는 횟수
    private const int RELOAD_BULLET_CHECK = 4;

    public void OnEnable()
    {
        SkillType   = SkillTypes.Primary;
        Cooldown    = COOLDOWN;
        SkillDamage = DAMAGE_MULTIPLIER;
    }

    public override bool Play()
    {
        if (!CanUse()) return false;
        if (!ConsumeResource()) return false;

        Debug.Log("RevolverShot");
        Transform _cachedTransform = transform;
        Vector3   _position        = _cachedTransform.position;

        RaycastHit2D _hit = Physics2D.Raycast(_position, _cachedTransform.right * (int)Facing, RANGE);

        Collider2D _hitCollider = _hit.collider;


        if (_hitCollider != null)
        {
            StartCoroutine(VFXManager.PlayVFX("BulletPop", _hit.point));
            if (_hitCollider.CompareTag("Enemy"))
            {
                int _damage = GetDamage();
                _hitCollider.GetComponent<EnemyBase>().Attacked(_damage, STUN_DURATION, Player);
            }
        }

        if (Stats.Resource == 0)
            Reload();

        LastUsedTime = Time.time;
        return true;
    }

    public static bool Reload()
    {
        Player _player = GameManager.Instance.GetLocalPlayer();
        _player.PlayStatusFeedback("RELOAD");
        Task.Run(() =>
        {
            for (int i = 0; i < RELOAD_BULLET_CHECK; i++)
            {
                if (_player.Stats.Resource > 0) return; //총알이 장전되었다면 재장전을 끊는다.

                Thread.Sleep((int)(RELOAD_TIME_SECONDS / RELOAD_BULLET_CHECK * 1000));
            }
            
            // 위에서 Resource가 0이 아닐때 즉시 return을 했으므로, 아래 if는 false가 된다.
            if (_player.Stats.Resource <= 0) //총알이 장전되지 않았다면 장전한다.
                _player.Stats.Resource = UIElements.Instance.resourceBar.value = _player.Stats.maxResource;
        });

        return false;
    }
}