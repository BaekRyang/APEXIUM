using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public static class GameManager
{
    public static bool isPlayInSingleMode = true;

#region CapsuleReward

    public const float REWARD_CAPSULE_RESOURCE = 10;
    public const float REWARD_CAPSULE_EXP      = 10;

#endregion

#region ChestCosts

    public static string GetCostString(int _cost)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedStringAsync("ChestCostIndicator").Result + _cost;
    }
    private static Dictionary<ChestType, float> ChestCosts = new()
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

    public const float COST_MULTIPLIER_LEVEL = 1.5f;

    public static int GetChestCost(ChestType _chestType)
    {
        return Mathf.RoundToInt(ChestCosts[_chestType] * Mathf.Pow(COST_MULTIPLIER_LEVEL, DifficultyManager.NowDifficulty));
    }
#endregion
}