using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public static class GameManager
{
    public static bool isPlayInSingleMode = true;

#region CapsuleReward

    private const float REWARD_CAPSULE_RESOURCE = 10;
    private const float REWARD_CAPSULE_EXP      = 10;

    private const float REWARD_MULTIPLIER_LEVEL = 1.5f;
    private const float REWARD_RANDOM_RANGE     = 0.1f;

    public static int GetRandomCapsuleReward(PickupType _pickupType)
    {
        if (_pickupType == PickupType.Item) return -1;

        return Mathf.RoundToInt(
            (_pickupType == PickupType.Resource ? REWARD_CAPSULE_RESOURCE : REWARD_CAPSULE_EXP) *
            Mathf.Pow(REWARD_MULTIPLIER_LEVEL, DifficultyManager.NowDifficulty)                 *
            (1 + Random.Range(-REWARD_RANDOM_RANGE, REWARD_RANDOM_RANGE))
        );
    }

#endregion

#region ChestCosts

    private static readonly Dictionary<ChestType, float> ChestCosts = new()
                                                                      {
                                                                          { ChestType.Capsule, -1 },
                                                                          { ChestType.Small, COST_CHEST_SMALL_RESOURCE },
                                                                          { ChestType.Medium, COST_CHEST_MEDIUM_RESOURCE },
                                                                          { ChestType.Large, COST_CHEST_LARGE_RESOURCE },
                                                                          { ChestType.Legendary, COST_CHEST_LEGENDARY_RESOURCE },
                                                                      };

    private const float COST_CHEST_SMALL_RESOURCE     = 20;
    private const float COST_CHEST_MEDIUM_RESOURCE    = 50;
    private const float COST_CHEST_LARGE_RESOURCE     = 100;
    private const float COST_CHEST_LEGENDARY_RESOURCE = 200;

    public static int GetRandomChestCost(ChestType _chestType)
    {
        if (_chestType == ChestType.Capsule) return -1;

        return Mathf.RoundToInt(
            ChestCosts[_chestType]                                            *
            Mathf.Pow(COST_MULTIPLIER_LEVEL, DifficultyManager.NowDifficulty) *
            (1 + Random.Range(-COST_RANDOM_RANGE, COST_RANDOM_RANGE))
        );
    }

    private const float COST_MULTIPLIER_LEVEL = 1.5f;

    private const float COST_RANDOM_RANGE = 0.05f;

#endregion

#region ChestItemRatio

    public static Vector4 RATIO_CHEST_SMALL     = new(0.60f, 0.30f, 0.10f, 0.00f);
    public static Vector4 RATIO_CHEST_MEDIUM    = new(0.20f, 0.53f, 0.19f, 0.08f);
    public static Vector4 RATIO_CHEST_LARGE     = new(0.10f, 0.45f, 0.30f, 0.15f);
    public static Vector4 RATIO_CHEST_LEGENDARY = new(0.00f, 0.30f, 0.36f, 0.29f);
    
#endregion
}