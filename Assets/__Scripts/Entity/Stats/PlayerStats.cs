using System;
using UnityEngine.Serialization;

[Serializable]
public class PlayerStats : Stats
{
    public  int   maxJumpCount = 5;
    public  float jumpHeight   = 10f;

    public PlayerStats()
    {
        Health       = 100;
    }
}