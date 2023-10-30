using System;

public static class SkillFactory
{
    public static Skill MakeSkill(string _skillName, Player _player)
    {
        Skill _skill = (Skill)Activator.CreateInstance(Type.GetType(_skillName)!);
        DIContainer.Inject(_skill.Initialize(_player));
        return _skill;
    }
}