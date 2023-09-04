using System;
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
}
