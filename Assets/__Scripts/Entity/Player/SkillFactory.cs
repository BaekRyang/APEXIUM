using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class SkillFactory
{
    public static Skill MakeSkill(string _skillName, Player _player)
    {
        Skill skill = (Skill)Activator.CreateInstance(Type.GetType(_skillName));
        skill.Player = _player;
        skill.Initialize();
        return skill;
    }

    // private static async void UpdateCooltimeUI()
    // {
    //     while (true)
    //     {
    //         foreach ((SkillTypes _skillTypes, Skill _skill) in _skills)
    //         {
    //             Debug.Log(_skill.GetType().ToString());
    //             if (_skill.Cooldown == 0) continue; //쿨타임 없으면 업데이트 안함
    //             
    //         }
    //
    //
    //         await UniTask.Delay(10);
    //     }
    // }
}