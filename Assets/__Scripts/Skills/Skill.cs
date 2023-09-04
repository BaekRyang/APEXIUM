using System;
using UnityEngine;

public class Skill : IUseable
{
    public    Player      Player { get; set; }
    protected PlayerStats Stats  => Player.Stats;

    public virtual void Initialize()
    {
    }

    public SkillTypes SkillType { get; protected set; }

    public float Cooldown { get; protected set; } //베이스 쿨타임(수정되면 안됨)

    public bool IsReady => RemainingCooldown <= 0;

    public float RemainingCooldown
    {
        get
        {
            float _remainingCooldown;
            if (SkillType == SkillTypes.Primary)
                _remainingCooldown = Cooldown / Stats.AttackSpeed - (Time.time - LastUsedTime);
            else
                _remainingCooldown = Cooldown - (Time.time - LastUsedTime);

            /*쿨타임 Modifier가 있을때만 작동하도록 해야함 -- 아니면 Cooldown 프로퍼티를 수정하는 방법도 있을듯*/
            // if (SkillType != SkillTypes.Primary && _remainingCooldown < .5f) 
            //     _remainingCooldown = .5f; //최소 쿨타임 제한 .5초 (메인 스킬은 제외)

            return _remainingCooldown;
        }

        set => LastUsedTime = Time.time + value - Cooldown; //설정한 값으로 쿨타임 변경
    }

    public float RealCooldown
    {
        get
        {
            float _realCooldown;
            if (SkillType == SkillTypes.Primary)
                _realCooldown = Cooldown / Stats.AttackSpeed;
            else
                _realCooldown = Cooldown;

            return _realCooldown;
        }
    }

    public float LastUsedTime { get; set; } = float.MinValue; //최소값 안쓰면 쿨타임이 돌아가는 상태로 시작함

    public virtual bool Play() => false;

    public virtual void Update()
    {
        if (RealCooldown > .1f) //너무 짧은 쿨타임은 보여주지 않음
            UIElements.Instance.SetCoolDown(SkillType, RealCooldown, RemainingCooldown);
        Debug.Log($"{LastUsedTime} -- {RemainingCooldown}");
    }

    protected bool CanUse()
    {
        if (!IsReady) return false;                        //쿨타임 체크
        if (!Player.Controller.Controllable) return false; //사용 가능한 상태인지 체크

        return true;
    }

    protected bool ConsumeResource(int p_amount = 1)
    {
        if (Stats.Resource >= p_amount) //자원이 충분한지 체크
        {
            Stats.Resource -= p_amount;
            return true;
        }

        return false;
    }
}