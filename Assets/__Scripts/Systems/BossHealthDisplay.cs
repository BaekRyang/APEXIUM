using MoreMountains.Tools;
using UnityEngine;

public class BossHealthDisplay : MonoBehaviour
{
    [SerializeField] private MMProgressBar _bar;
    
    public void SetHealth(float health)
    {
        _bar.UpdateBar01(health);
    }
}
