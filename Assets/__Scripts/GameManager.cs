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

    [DoNotSerialize] private readonly Dictionary<int, Player>        _players        = new();
    [DoNotSerialize] private readonly Dictionary<string, PlayerData> _charactersData = new();
    [DoNotSerialize] private readonly Dictionary<string, EnemyData>  _monstersData   = new();
    
    public CinemachineVirtualCamera virtualCamera;

    public PlayMap currentMap;
    
    public Player GetRandomPlayer()
    {
        if (_players.Count <= 0)
            return null;

        return _players[Random.Range(0, _players.Count)];
    }

    public Player[] GetPlayers() => _players.Values.ToArray();

    public Player GetLocalPlayer() => _players[playerID];

    public PlayerData GetCharacterData(string p_name) => _charactersData[p_name];
    public EnemyData  GetEnemyData(string     p_name) => _monstersData[p_name];

    private void Awake()
    {
        Instance ??= this;
        DontDestroyOnLoad(gameObject);

        foreach (var _character in characters)
            _charactersData.Add(_character.name, _character);

        foreach (var _monster in monsters)
            _monstersData.Add(_monster.name, _monster);


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
        _players.Add(p_newPlayerID, _player.GetComponent<Player>());
    }

    public void RemovePlayer(int p_playerID)
    {
        Destroy(_players[p_playerID].gameObject);
        _players.Remove(p_playerID);
    }
}