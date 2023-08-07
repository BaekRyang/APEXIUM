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
        Revolver.NextReloadTime = Revolver.GetNextReloadTime();

        Debug.Log("RevolverShot");
        Player._animator.SetTrigger("Primary");
        Player._animator.SetBool("Playing", true);
        Transform _cachedTransform = transform;
        Vector3   _position        = _cachedTransform.position;

        LayerMask    _layerMask = LayerMask.GetMask("Enemy", "Floor");
        RaycastHit2D _hit       = Physics2D.Raycast(_position, _cachedTransform.right * (int)Facing, RANGE, _layerMask);

        Collider2D _hitCollider = _hit.collider;

        if (_hitCollider != null)
        {
            StartCoroutine(VFXManager.PlayVFX("BulletPop", _hit.point, (int)Player.Controller.PlayerFacing));
            if (_hitCollider.CompareTag("Enemy"))
            {
                int _damage = GetDamage();
                _hitCollider.GetComponent<EnemyBase>().Attacked(_damage, STUN_DURATION, Player);
            }
        }
    
        Player.Controller.AddLandingAction(() => Player.Controller.Rigidbody2D.velocity = Vector2.zero);

        // Task.Run(async () =>
        // {
        //     // Thread.Sleep(200);
        //     await Task.Delay(200);
        //     Player.Controller.SetControllable(true);
        // });
        
        StartCoroutine(Revolver.DelayAndSetControllable(Player));

        if (Stats.Resource == 0)
            Revolver.Reload();

        LastUsedTime = Time.time;
        return true;
    }

}