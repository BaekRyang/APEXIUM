public interface IUseable
{
    public Player     Player            { get; }
    public float      Cooldown          { get; }
    public float      RemainingCooldown { get; }
    public float      LastUsedTime      { get; set; }

    public void Play();
    public void Update();
}