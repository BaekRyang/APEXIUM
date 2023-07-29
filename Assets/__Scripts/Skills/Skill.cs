using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour, IUseable
{
    private void Start() => Player = GetComponent<Player>();

    public    Player      Player { get; private set; }
    protected PlayerStats Stats  => Player.Stats;

    public SkillTypes SkillType { get; protected set; }

    public float Cooldown { get; protected set; } //베이스 쿨타임(수정되면 안됨)

    public float RemainingCooldown
    {
        get
        {
            float _remainingCooldown = Cooldown - (Time.time - LastUsedTime);

            /*쿨타임 Modifier가 있을때만 작동하도록 해야함 -- 아니면 Cooldown 프로퍼티를 수정하는 방법도 있을듯*/
            // if (SkillType != SkillTypes.Primary && _remainingCooldown < .5f) 
            //     _remainingCooldown = .5f; //최소 쿨타임 제한 .5초 (메인 스킬은 제외)

            return _remainingCooldown;
        }

        set => LastUsedTime = Time.time + value - Cooldown; //설정한 값으로 쿨타임 변경
    }

    public float LastUsedTime { get; set; }

    public virtual bool Play() => false;

    private void Update()
    {
        // if (SkillType != SkillTypes.Primary) //쿨타임 UI 업데이트
            UIElements.Instance.SetCoolDown(SkillType, RemainingCooldown);
    }

    protected bool CanUse()
    {
        if (RemainingCooldown > 0) return false;           //쿨타임 체크
        if (!Player.Controller.controllable) return false; //사용 가능한 상태인지 체크

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