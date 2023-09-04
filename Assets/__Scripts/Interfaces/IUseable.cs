public interface IUseable
{
    public Player     Player            { get; }
    public SkillTypes SkillType         { get; }
    public float      Cooldown          { get; }
    public float      RemainingCooldown { get; }
    public float      LastUsedTime      { get; set; }

    public bool Play();
    public void Update();
}