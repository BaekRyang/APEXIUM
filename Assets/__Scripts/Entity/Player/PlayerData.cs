using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    [Header("Name")]
    public string characterName;

    [Space] [Header("Stats")]
    public PlayerStats stats;

    [Space] [Header("Skills")]
    public string skillPassive;
    public string skillPrimary;
    public string skillSecondary;
    public string skillUtility;
    public string skillUltimate; }

public class TestEvent
{
    public event EventHandler OnTestEvent;
    
    public void Test()
    {
        OnTestEvent?.Invoke(this, EventArgs.Empty);
    }
}