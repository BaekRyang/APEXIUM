using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Facing = PlayerController.Facing;
using Random = UnityEngine.Random;

public class AttackableSkill : Skill, IAttackable
{
    protected Facing Facing      => Player.Controller.PlayerFacing;
    private   float  StatDamage  => GameManager.Instance.GetLocalPlayer().Stats.attackDamage;
    public    float  SkillDamage { get; set; }

    protected int GetDamage()
    {
        //0.8에서 1.2 사이의 값 생성 소수 둘째자리까지
        float _randomDamageMultiplier = Random.Range(0.8f, 1.2f);
        float _damage = StatDamage              *
                        _randomDamageMultiplier *
                        SkillDamage;

        return (int)_damage;
    }
}