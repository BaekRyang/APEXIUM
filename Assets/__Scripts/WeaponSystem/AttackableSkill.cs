using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facing = PlayerController.Facing;
using Random = UnityEngine.Random;

public class AttackableSkill : Skill
{
    protected float      skillDamage = 1f;
    
    protected Facing Facing     => player.Controller.PlayerFacing;
    private   float  StatDamage => GameManager.Instance.GetLocalPlayer().Stats.attackDamage;
    
    public int GetDamage()
    {
        //0.8에서 1.2 사이의 값 생성 소수 둘째자리까지
        float _randomDamageMultiplier = Random.Range(0.8f, 1.2f);
        float _damage = StatDamage              *
                        _randomDamageMultiplier *
                        skillDamage;

        return (int)_damage;
    }
}