using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
}
