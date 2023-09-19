using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Roll : Skill, IUseable
{
    [Inject] private CameraManager _cameraManager;
    
    private const float POWER    = 1.85f;
    private const float EVADE_TIME = 0.5f;
    private const float COOLDOWN = 3f;

    public override Skill Initialize(Player _player)
    {
        base.Initialize(_player);
        
        SkillType = SkillTypes.Utility;
        Cooldown  = COOLDOWN;
        
        return this;
    }
    
    public void Play()
    {
        if (!CanUse()) return;

        //바라보고 있는 방향으로 구르기
        Player.Controller.SetControllable(false);

        var _force = (int)Player.Controller.PlayerFacing * //방향
                     POWER                               * //스킬 기본 속도
                     Player.Stats.Speed                  * //플레이어 속도
                     Vector2.right;
        
        Player.Controller.Rigidbody2D.AddForce(_force, ForceMode2D.Impulse);
        //AddForce를 사용하면 플레이어가 점프를 했을때 속도가 크게 증가하는 현상이 있어서 속도 제한
        if (Mathf.Abs(Player.Controller.Rigidbody2D.velocity.x) > Player.Stats.Speed * POWER + .01f) //플레이어의 속도가 의도보다 높다면
            Player.Controller.Rigidbody2D.velocity =  //의도한 속도 + 20% 정도로 제한
                new Vector2(Player.Stats.Speed                  * POWER * 
                            (int)Player.Controller.PlayerFacing * 1.2f,
                            Player.Controller.Rigidbody2D.velocity.y);
        
        Player._animator.SetTrigger("Roll");
        Task.Run(() =>
        {
            Thread.Sleep((int)(EVADE_TIME * 1000));
            Player.Controller.SetControllable(true);
        });

        Player.Stats.Resource = Player.Stats.MaxResource;

        //TODO: 업그레이드 효과?
        Player.skills[SkillTypes.Secondary].RemainingCooldown = 0;
        Player.skills[SkillTypes.Ultimate].RemainingCooldown  = 0;

        LastUsedTime = Time.time;
    }
}