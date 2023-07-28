using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    protected Player     player;
    protected SkillTypes skillType;
    
    protected float      cooldown;
    private   float      _lastUsedTime;
    private   float      _remainingCooldown;
    
    protected void Start() => player = GetComponent<Player>();
    
    public virtual bool Play()
    {
        if (_remainingCooldown > 0) return false;

        _lastUsedTime = Time.time;

        return true;
    }
    
    private void Update()
    {
        _remainingCooldown = cooldown - (Time.time - _lastUsedTime);

        if (skillType != SkillTypes.Primary)
            UIElements.Instance.skillBlocks[skillType].SetCoolDown(_remainingCooldown);
    }
}
