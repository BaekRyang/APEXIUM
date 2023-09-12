using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [SerializeField] private PlayerData[] characters;
    [SerializeField] private EnemyData[]  monsters;

    [DoNotSerialize] private static readonly Dictionary<string, PlayerData> CharactersData = new();
    [DoNotSerialize] private static readonly Dictionary<string, EnemyData>  MonstersData   = new();

    public static PlayerData GetCharacterData(string _name) => CharactersData[_name];
    public static EnemyData  GetEnemyData(string     _name) => MonstersData[_name];

    private void Awake()
    {
        foreach (PlayerData _character in characters)
            CharactersData.Add(_character.name, _character);

        foreach (EnemyData _monster in monsters)
            MonstersData.Add(_monster.name, _monster);
    }
}