using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int playerID = -1;

    public Statistics statistics;

    public CellSlider _cellSlider;

    [SerializeField] private Dictionary<int, Player> players = new Dictionary<int, Player>();
    [SerializeField] private GameObject              playerPrefab;

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
    }

    public void InstantiatePlayer(int p_newPlayerID)
    {
        GameObject _player = Instantiate(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        _player.transform.name = $"Player {p_newPlayerID}";

        _player.GetComponent<Player>().clientID = p_newPlayerID;
        players.Add(p_newPlayerID, _player.GetComponent<Player>());
    }
}