using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour, IAttackable
{
    protected Player player;

    protected void  Start() => player = GetComponent<Player>();
    protected int   Facing  => player.Controller.PlayerFacing;
    public    float Damage  => GameManager.Instance.GetLocalPlayer().Stats.attackDamage;

    public abstract void Play(int p_damageMultiplier);
}