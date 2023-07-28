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
        skillType = SkillTypes.Utility;
        cooldown  = COOLDOWN;
    }

    public override bool Play()
    {
        if (!base.Play()) return false;

        //바라보고 있는 방향으로 구르기
        player.Controller.controllable = false;

        var _force = (int)player.Controller.PlayerFacing * //방향
                     POWER                               * //스킬 기본 속도
                     player.Stats.speed                  * //플레이어 속도
                     Vector2.right;
        
        player.Controller.Rigidbody2D.AddForce(_force, ForceMode2D.Impulse);
        Task.Run( () =>
        {
            Thread.Sleep(500);
            player.Controller.controllable = true;
        });
        return true;
    }
}