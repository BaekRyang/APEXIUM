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

    [SerializeField] private Dictionary<int, Player> players = new Dictionary<int, Player>();
    [SerializeField] private GameObject              playerPrefab;

    [SerializeField] private                PlayerData[]                   characters;
    [SerializeField] private                EnemyData[]                    monsters;
    [DoNotSerialize] public static readonly Dictionary<string, PlayerData> CharactersData = new();
    [DoNotSerialize] public static readonly Dictionary<string, EnemyData>  MonstersData   = new();

    public CinemachineVirtualCamera virtualCamera;

    public Player GetRandomPlayer()
    {
        if (players.Count <= 0)
            return null;

        return players[Random.Range(0, players.Count)];
    }

    public Player[] GetPlayers()
    {
        return players.Values.ToArray();
    }

    public Player GetLocalPlayer()
    {
        return players[playerID];
    }

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
        var _spawnPosition = Physics2D.RaycastAll(new Vector2(0, 100), Vector2.down, 100, LayerMask.GetMask("Floor"))[^1].point;
        _spawnPosition.y += 1.5f;
        GameObject _player = Instantiate(playerPrefab, _spawnPosition, Quaternion.identity);
        _player.transform.name = $"Player {p_newPlayerID}";

        _player.GetComponent<Player>().clientID = p_newPlayerID;
        players.Add(p_newPlayerID, _player.GetComponent<Player>());
    }
}