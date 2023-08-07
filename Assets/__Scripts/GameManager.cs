using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cinemachine;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

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

    public Player RandomPlayer()
    {
        if (players.Count <= 0)
            return null;

        return players[Random.Range(0, players.Count)];
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
        GameObject _player = Instantiate(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        _player.transform.name = $"Player {p_newPlayerID}";

        _player.GetComponent<Player>().clientID = p_newPlayerID;
        players.Add(p_newPlayerID, _player.GetComponent<Player>());
    }
}