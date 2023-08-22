using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int playerID = -1;

    public Statistics statistics;

    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private PlayerData[] characters;
    [SerializeField] private EnemyData[]  monsters;

    [DoNotSerialize] private readonly Dictionary<int, Player>        Players        = new();
    [DoNotSerialize] private readonly Dictionary<string, PlayerData> CharactersData = new();
    [DoNotSerialize] private readonly Dictionary<string, EnemyData>  MonstersData   = new();

    public CinemachineVirtualCamera virtualCamera;

    public Player GetRandomPlayer()
    {
        if (Players.Count <= 0)
            return null;

        return Players[Random.Range(0, Players.Count)];
    }

    public Player[] GetPlayers() => Players.Values.ToArray();

    public Player GetLocalPlayer() => Players[playerID];

    public PlayerData GetCharacterData(string p_name) => CharactersData[p_name];
    public EnemyData  GetEnemyData(string     p_name) => MonstersData[p_name];

    private void Awake()
    {
        Instance ??= this;
        DontDestroyOnLoad(gameObject);

        foreach (var _character in characters)
            CharactersData.Add(_character.name, _character);

        foreach (var _monster in monsters)
            MonstersData.Add(_monster.name, _monster);
    }

    public void InstantiatePlayer(int p_newPlayerID)
    {
        //RaycastAll로 가장 마지막에 충돌한 오브젝트의 위치를 가져옴
        //RaycastNonAlloc이 성능상 더 좋아보이긴 하지만 단발적으로 사용할거라 큰 차이는 없을듯
        var _spawnPosition = Physics2D.RaycastAll(new(0, 100), Vector2.down, 100, LayerMask.GetMask("Floor"))[^1].point;
        _spawnPosition.y += 1.5f;
        GameObject _player = Instantiate(playerPrefab, _spawnPosition, Quaternion.identity);
        _player.transform.name = $"Player {p_newPlayerID}";

        _player.GetComponent<Player>().clientID = p_newPlayerID;
        Players.Add(p_newPlayerID, _player.GetComponent<Player>());
    }

    public void RemovePlayer(int p_playerID)
    {
        Destroy(Players[p_playerID].gameObject);
        Players.Remove(p_playerID);
    }
}