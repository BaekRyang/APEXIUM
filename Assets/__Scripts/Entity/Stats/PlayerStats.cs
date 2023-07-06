public class PlayerStats : Stats
{
    public new float Speed { get; set; } = 4f;
    
    public int MaxJumpCount { get; set; } = 1;

    public float AttackSpeed { get; set; } = 1;

    public float JumpHeight { get; set; } = 7.5f;
}
