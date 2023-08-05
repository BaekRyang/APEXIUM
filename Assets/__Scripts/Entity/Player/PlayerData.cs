using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    [Header("Name")]
    [SerializeField] private string _characterName;

    [Space] [Header("Stats")]
    [SerializeField] private PlayerStats _stats;

    [Space] [Header("Skills")]
    [SerializeField] private String _skill_Passive;
    [SerializeField] private String _skill_Primary;
    [SerializeField] private String _skill_Secondary;
    [SerializeField] private String _skill_Utility;
    [SerializeField] private String _skill_Ultimate;
}