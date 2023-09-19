using UnityEngine;

public class ItemSkill : Skill, IUseable
{
    public override Skill Initialize(Player _player)
    {
        SkillType = SkillTypes.Item;
        Cooldown  = -1;
        
        return this;
    }

    public void Play()
    {
        //플레이어가 들고있는 아이템을 가져와서 Play
        throw new System.NotImplementedException();
    }
}