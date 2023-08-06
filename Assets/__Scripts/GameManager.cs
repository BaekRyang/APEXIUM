using System.Collections.Generic;
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
    [DoNotSerialize] public static readonly Dictionary<string, PlayerData> CharactersData = new();

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
    }

    public void InstantiatePlayer(int p_newPlayerID)
    {
        GameObject _player = Instantiate(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        _player.transform.name = $"Player {p_newPlayerID}";

        _player.GetComponent<Player>().clientID = p_newPlayerID;
        players.Add(p_newPlayerID, _player.GetComponent<Player>());
    }
}