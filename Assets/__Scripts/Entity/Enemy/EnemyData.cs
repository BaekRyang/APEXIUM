using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData", order = 1)]
public class EnemyData : ScriptableObject
{
    [Header("Name")]
    public string enemyName;

    [Space] [Header("Stats")]
    public EnemyStats stats;
    public Vector4 attackColliderOffsets;

    [Space] [Header("Is Boss")]
    public bool isBoss;

    [Space] [Header("Sprites")]
    public Sprite sprite;
    public RuntimeAnimatorController animatorController;

}