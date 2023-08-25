using System;
using MoreMountains.Tools;
using UnityEngine;

public class BossHealthDisplay : MonoBehaviour
{
    public static BossHealthDisplay Instance;

    [SerializeField] private MMProgressBar bar;

    private EnemyBase _syncedEnemyBase;
    private int       _priority;

    private void Awake() => Instance ??= this;

    private void UpdateHealth()
    {
        bar.UpdateBar(Math.Max(0, _syncedEnemyBase.stats.Health), 0, _syncedEnemyBase.stats.MaxHealth);
    }

    public void SyncToBossHealthBar(EnemyBase p_enemyBase, int p_priority = 0)
    {
        if (_priority > p_priority) return;

        _syncedEnemyBase = p_enemyBase;

        bar.TextValueMultiplier = p_enemyBase.stats.MaxHealth;
        _priority               = p_priority;

        _syncedEnemyBase.OnEnemyHpChange += OnHealthChange;

        bar.SetBar01(1) ;
    }

    public void UnSyncToBossHealthBar(EnemyBase p_enemyBase)
    {
        if (_syncedEnemyBase != p_enemyBase) return;

        _syncedEnemyBase.OnEnemyHpChange -= OnHealthChange;
        _syncedEnemyBase                 =  null;
        _priority                        =  -1;
    }

    private void OnHealthChange(object p_sender, EventArgs p_args)
    {
        Debug.Log("ACTIVATE");
        if (_syncedEnemyBase != (EnemyBase)p_sender) return;
        UpdateHealth();
    }
}