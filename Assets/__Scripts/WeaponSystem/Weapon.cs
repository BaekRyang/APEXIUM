using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facing = PlayerController.Facing;

public class Weapon : MonoBehaviour, IAttackable
{
    protected Player player;
    protected float? cooldown;
    private   float  _lastAttackTime;

    protected void   Start() => player = GetComponent<Player>();
    protected Facing Facing  => player.Controller.PlayerFacing;
    protected float  Damage  => GameManager.Instance.GetLocalPlayer().Stats.attackDamage;

    public virtual bool Play(int p_damageMultiplier)
    {
        if (Time.time - _lastAttackTime < cooldown) return false; //쿨타임
        _lastAttackTime = Time.time;
        return true;
    }
}