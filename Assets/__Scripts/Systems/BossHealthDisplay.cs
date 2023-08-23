using MoreMountains.Tools;
using UnityEngine;

public static class BossHealthDisplay
{
    private static MMProgressBar Bar;
    
    public static void SetHealthByPercent(float p_health)
    {
        Bar.UpdateBar01(p_health);
    }

    private static void Initialize(int p_maxHealth)
    {
        Bar.TextValueMultiplier = p_maxHealth;
    }

    // public static void SyncToBossHealthBar(this EnemyBase p_enemyBase)
    // {
    //     
    // }

}
