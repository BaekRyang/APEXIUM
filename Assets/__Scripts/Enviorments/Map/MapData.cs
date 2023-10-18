using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapData : MonoBehaviour
{
    public Transform     sceneObjects;
    public Transform     background;
    public PlayMap       currentMap;
    public ObjectPrefabs objectPrefabs;

    public List<SpawnRatio<ChestType>> spawnRatio = new()
                                                    {
                                                        new SpawnRatio<ChestType>(ChestType.Small,     60),
                                                        new SpawnRatio<ChestType>(ChestType.Medium,    25),
                                                        new SpawnRatio<ChestType>(ChestType.Large,     10),
                                                        new SpawnRatio<ChestType>(ChestType.Legendary, 5)
                                                    };

    public List<EnemyData> spawnableEnemies;

    public ChestType GetRandomChest()
    {
        return spawnRatio.GetRandomKey();
    }

    public GameObject GetRandomChestGameObject(ChestType _chestType)
    {
        GameObject _obj = objectPrefabs.GetObject(_chestType);
        Debug.Log("<color=red>" + _obj.name + "</color>");
        return _obj;
    }
}

[Serializable]
public struct ObjectPrefabs
{
    public GameObject capsule, small, medium, large, legendary;

    public GameObject GetObject(ChestType _chestType) => _chestType switch
    {
        ChestType.Capsule   => capsule,
        ChestType.Small     => small,
        ChestType.Medium    => medium,
        ChestType.Large     => large,
        ChestType.Legendary => legendary,
        _                   => throw new ArgumentOutOfRangeException(nameof(_chestType), _chestType, null)
    };
}

public enum ChestType
{
    Capsule,
    Small,
    Medium,
    Large,
    Legendary
}

[Serializable]
public class SpawnRatio<T>
{
    public T     key;
    public float ratio;

    public SpawnRatio(T _key, float _ratio)
    {
        key   = _key;
        ratio = _ratio;
    }
}

public static class SpawnRatioExt
{
    public static T GetRandomKey<T>(this List<SpawnRatio<T>> _list)
    {
        float _totalRatio = 0;

        foreach (SpawnRatio<T> _ratio in _list)
            _totalRatio += _ratio.ratio;

        float _randomRatio = Random.Range(0, _totalRatio);

        foreach (SpawnRatio<T> _ratio in _list)
        {
            _randomRatio -= _ratio.ratio;
            if (_randomRatio <= 0)
                return
                    _ratio.key;
        }

        return default;
    }
}