using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Revolver : Skill
{
    private const float RELOAD_TIME_SECONDS         = 2f;
    private const int   RELOAD_BULLET_CHECK         = 4; //재장전중 총알이 장전되었다면 재장전을 끊기 위하여 총알이 장전되었는지 확인하는 횟수
    private const float AUTO_RELOAD_TIME_MULTIPLIER = 1.5f;

    public static float NextReloadTime;

    public override void Initialize()
    {
        SkillType = SkillTypes.Passive;
    }

    public override void Update()
    {
        base.Update();
        if (Player.Stats.Resource == Player.Stats.MaxResource) return; //재장전이 되었으면 리턴

        //총알이 다 채워지지 않은 상태로 재장전시간 +50% 초만큼 지나면 총알을 채운다.
        if (Time.time > NextReloadTime)
        {
            Player.Stats.Resource = Player.Stats.MaxResource;
            NextReloadTime        = GetNextReloadTime();
        }
    }

    public static float GetReloadTime()
    {
        Player _player = GameManager.Instance.GetLocalPlayer();
        return RELOAD_TIME_SECONDS / _player.Stats.AttackSpeed; //공격속도에 따라 재장전 시간이 달라진다.
    }

    public static void Reload()
    {
        Player _player = GameManager.Instance.GetLocalPlayer();
        _player.PlayStatusFeedback("RELOAD");

        //재장전중에는 자동 재장전을 끊는다.
        NextReloadTime = float.MaxValue;

        UniTask.Void(async () =>
        {
            for (int i = 0; i < RELOAD_BULLET_CHECK; i++)
            {
                if (_player.Stats.Resource > 0) return; //총알이 장전되었다면 재장전을 끊는다.

                await UniTask.Delay((int)(GetReloadTime() / RELOAD_BULLET_CHECK * 1000));
            }

            // 위에서 Resource가 0이 아닐때 즉시 return을 했으므로, 아래 if는 false가 된다.
            if (_player.Stats.Resource <= 0) //총알이 장전되지 않았다면 장전한다.
                _player.Stats.Resource = _player.Stats.MaxResource;
        });
    }

    public static float GetNextReloadTime()
    {
        return Time.time + GetReloadTime() * AUTO_RELOAD_TIME_MULTIPLIER;
    }

    public static IEnumerator DelayAndSetControllable(Player p_player, float p_delay)
    {
        p_player.Controller.SetControllable(false);
        yield return new WaitForSeconds(p_delay);
        p_player.Controller.SetControllable(true);
    }

    public static void CachingData(Player _player, out Transform _cachedTransform, out Vector3 _position)
    {
        _cachedTransform = _player.Controller.attackPosTransform;
        _position        = _cachedTransform.position;
    }
}