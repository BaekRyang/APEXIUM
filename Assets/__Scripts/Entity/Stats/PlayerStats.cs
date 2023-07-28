using System;
using UnityEngine.Serialization;

[Serializable]
public class PlayerStats : Stats<PlayerStats>
{
    public  int   maxJumpCount;
    public  float jumpHeight;
    
    public PlayerStats SetMaxJumpCount(int p_maxJumpCount)
    {
        maxJumpCount = p_maxJumpCount;
        return this;
    }
    
    public PlayerStats SetJumpHeight(float p_jumpHeight)
    {
        jumpHeight = p_jumpHeight;
        return this;
    }
}