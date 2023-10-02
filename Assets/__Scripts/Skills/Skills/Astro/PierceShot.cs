using UnityEngine;

public class PierceShot : AttackableSkill
{
    private const float RANGE         = 50f;
    private const float COOLDOWN      = 6f;
    private const float STUN_DURATION = 1f;
    private const float SKILL_DAMAGE  = 3f;

    private Revolver _revolver;

    public override Skill Initialize(Player _player)
    {
        base.Initialize(_player);
        
        SkillType   = SkillTypes.Secondary;
        Cooldown    = COOLDOWN;
        SkillDamage = SKILL_DAMAGE;

        _revolver = GetRevolver();
        
        return this;
    }

    private Revolver GetRevolver()
    {
        if (!Player.skills.TryGetValue(SkillTypes.Passive, out Skill _passive)) return null;
        if (_passive is Revolver _value)
            return _value;

        return null;
    }

    public override void Play()
    {
        if (!CanUse()) return;
        if (!ConsumeResource()) return;
        _revolver.NextReloadTime = _revolver.GetNextReloadTime();

        _revolver.CachingData(Player, out Transform _cachedTransform, out Vector3 _position);

        LayerMask      _layerMask = LayerMask.GetMask("Enemy", "Floor");
        RaycastHit2D[] _hit       = Physics2D.RaycastAll(_position, _cachedTransform.right * (int)Facing, RANGE, _layerMask);
        Player.StartCoroutine(_revolver.DelayAndSetControllable(.2f));
        foreach (RaycastHit2D _hitObject in _hit)
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
            _revolver.Reload();
            return;
        }

        LastUsedTime = Time.time;
    }
}