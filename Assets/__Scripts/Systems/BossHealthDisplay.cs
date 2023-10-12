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

    public void SyncToBossHealthBar(EnemyBase _enemyBase, int _priority = 0)
    {
        if (this._priority > _priority) return;

        _syncedEnemyBase = _enemyBase;

        bar.TextValueMultiplier = _enemyBase.stats.MaxHealth;
        this._priority          = _priority;

        _syncedEnemyBase.OnEnemyHpChange += OnHealthChange;

        bar.SetBar01(1) ;
    }

    public void UnSyncToBossHealthBar(EnemyBase _enemyBase)
    {
        if (_syncedEnemyBase != _enemyBase) return;

        _syncedEnemyBase.OnEnemyHpChange -= OnHealthChange;
        _syncedEnemyBase                 =  null;
        _priority                        =  -1;
    }

    private void OnHealthChange(object _sender, EventArgs _args)
    {
        if (_syncedEnemyBase != (EnemyBase)_sender) return;
        UpdateHealth();
    }
}