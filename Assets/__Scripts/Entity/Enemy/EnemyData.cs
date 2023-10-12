using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData", order = 1)]
public class EnemyData : ScriptableObject
{
    [Header("Name")]
    public string enemyName;

    [Space] [Header("Stats")]
    public EnemyStats stats;

    [Space] [Header("Is Boss")]
    public bool isBoss;

    [Space] [Header("Sprites")]
    public Sprite sprite;
    public AnimatorController animatorController;
}