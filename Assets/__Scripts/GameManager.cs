using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int playerID = -1;

    public Statistics statistics;

    [SerializeField] private List<Player> players;
    [SerializeField] private GameObject   playerPrefab;

    public Player RandomPlayer
    {
        get
        {
            if (players.Count <= 0)
                return null;

            return players[Random.Range(0, players.Count)];
        }
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
        players.Add(_player.GetComponent<Player>());
    }
}