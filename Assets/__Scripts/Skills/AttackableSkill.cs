using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Facing = PlayerController.Facing;
using Random = UnityEngine.Random;

public abstract class AttackableSkill : Skill, IAttackable, IUseable
{
    protected Facing Facing         => Player.Controller.PlayerFacing;
    private   float  StatDamage     => Player.Stats.AttackDamage;
    private   float  CriticalChance => Player.Stats.CriticalChance;
    private   float  CriticalDamage => Player.Stats.CriticalDamage;
    public    float  SkillDamage    { get; set; }

    protected (int, bool) GetDamage()
    {
        bool _isCritical = CriticalChance >= Random.Range(0f, 1f);

        //0.8에서 1.2 사이의 값 생성 소수 둘째자리까지
        float _randomDamageMultiplier = Random.Range(0.8f, 1.2f);
        float _damage = StatDamage              *
                        _randomDamageMultiplier *
                        SkillDamage             *
                        (_isCritical ?
                            CriticalDamage :
                            1f);


        return ((int)_damage, _isCritical);
    }

    public abstract void Play();
}