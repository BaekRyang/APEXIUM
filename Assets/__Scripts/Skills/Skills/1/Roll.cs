using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Roll : Skill
{
    private const float POWER    = 1.5f;
    private const float COOLDOWN = 2f;

    public void OnEnable()
    {
        SkillType = SkillTypes.Utility;
        Cooldown  = COOLDOWN;
    }

    public override bool Play()
    {
        if (!CanUse()) return false;

        //바라보고 있는 방향으로 구르기
        Player.Controller.controllable = false;

        var _force = (int)Player.Controller.PlayerFacing * //방향
                     POWER                               * //스킬 기본 속도
                     Player.Stats.speed                  * //플레이어 속도
                     Vector2.right;

        Player.Controller.Rigidbody2D.AddForce(_force, ForceMode2D.Impulse);
        Task.Run(() =>
        {
            Thread.Sleep(500);
            Player.Controller.controllable = true;
        });

       
        Stats.Resource = Stats.maxResource;

        GameManager.Instance.GetLocalPlayer().skills[SkillTypes.Secondary].RemainingCooldown = 0;
        GameManager.Instance.GetLocalPlayer().skills[SkillTypes.Ultimate].RemainingCooldown = 0;
        
        LastUsedTime = Time.time;
        return true;
    }
}