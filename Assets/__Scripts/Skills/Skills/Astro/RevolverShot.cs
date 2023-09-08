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
    private const float RANGE             = 40f;
    public const  float COOLDOWN          = .25f;
    private const float STUN_DURATION     = 0f;
    private const float DAMAGE_MULTIPLIER = 1f;


    public override void Initialize()
    {
        SkillType   = SkillTypes.Primary;
        Cooldown    = COOLDOWN;
        SkillDamage = DAMAGE_MULTIPLIER;
    }

    public override void Play()
    {
        if (!CanUse()) return;
        if (!ConsumeResource()) return;
        Revolver.NextReloadTime = Revolver.GetNextReloadTime();
        
        Player._animator.SetTrigger("Primary");
        Player._animator.SetBool("Playing", true);
        
        Revolver.CachingData(Player, out Transform _cachedTransform, out Vector3 _position);

        LayerMask    _layerMask = LayerMask.GetMask("Enemy", "Floor");
        RaycastHit2D _hit       = Physics2D.Raycast(_position, _cachedTransform.right * (int)Facing, RANGE, _layerMask);
        
        Collider2D _hitCollider = _hit.collider;

        if (_hitCollider != null)
        {
            VFXManager.PlayVFX("BulletPop", _hit.point, (int)Player.Controller.PlayerFacing);
            if (_hitCollider.CompareTag("Enemy"))
            {
                (int _damage, bool _critical) = GetDamage();
                _hitCollider.GetComponent<EnemyBase>().Attacked(_damage, _critical, STUN_DURATION, Player);
            }
        }

        Player.Controller.AddLandingAction(() => Player.Controller.Rigidbody2D.velocity = Vector2.zero);

        // Task.Run(async () =>
        // {
        //     // Thread.Sleep(200);
        //     await Task.Delay(200);
        //     Player.Controller.SetControllable(true);
        // });

        Player.StartCoroutine(Revolver.DelayAndSetControllable(Player, .2f / Player.Stats.AttackSpeed));

        if (Player.Stats.Resource == 0)
            Revolver.Reload();

        LastUsedTime = Time.time;
    }
}