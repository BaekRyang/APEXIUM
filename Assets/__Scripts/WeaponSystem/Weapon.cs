using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facing = PlayerController.Facing;

public class Weapon : MonoBehaviour, IAttackable
{
    protected Player player;
    protected float? cooldown;
    private   float  _lastAttackTime;
    protected float  skillDamage = 1f;

    protected void   Start()    => player = GetComponent<Player>();
    protected Facing Facing     => player.Controller.PlayerFacing;
    private   float  StatDamage => GameManager.Instance.GetLocalPlayer().Stats.attackDamage;

    public virtual bool Play()
    {
        if (Time.time - _lastAttackTime < cooldown) return false; //쿨타임
        _lastAttackTime = Time.time;
        return true;
    }

    protected int GetDamage()
    {
        //0.8에서 1.2 사이의 값 생성 소수 둘째자리까지
        float _randomDamageMultiplier = Random.Range(0.8f, 1.2f);
        float _damage = StatDamage              *
                        _randomDamageMultiplier *
                        skillDamage;

        return (int)_damage;
    }
}