using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

public static class SkillFactory
{
    public static Skill MakeSkill(string _skillName, Player _player)
    {
        Skill _skill = (Skill)Activator.CreateInstance(Type.GetType(_skillName)!);
        _skill.Player = _player;
        _skill.Initialize();
        return _skill;
    }
}