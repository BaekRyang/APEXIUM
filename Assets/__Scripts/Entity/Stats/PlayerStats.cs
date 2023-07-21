using System;

public class PlayerStats : Stats
{
    private    int _health = 100;

    public new int Health
    {
        get => _health;
        set
        {
            _health = value;
        }
    }

    public new float Speed { get; set; } = 4f;

    public int MaxJumpCount { get; set; } = 5;

    public float JumpHeight { get; set; } = 10f;
}
