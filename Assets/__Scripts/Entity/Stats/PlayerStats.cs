using System;

[Serializable]
public class PlayerStats : Stats<PlayerStats>
{
    public int   exp;
    public int   maxExp = 100;
    public int   maxJumpCount;
    public float jumpHeight;
    public int   resource;
    public int   maxResource;

    public int Resource
    {
        get => resource;
        set => UIElements.Instance.resourceBar.value = resource = value;
    }

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

    public PlayerStats SetResource(int p_resource)
    {
        UIElements.Instance.resourceBar.maxValue = UIElements.Instance.resourceBar.value = p_resource;
        UIElements.Instance.resourceBar.ApplySetting();
        Resource = maxResource = p_resource;
        return this;
    }
}