using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

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
        return _players.Count switch
        {
            <= 0 => null,
            _    => _players[Random.Range(0, _players.Count)]
        };
    }

    public IEnumerable<Player> GetPlayersArray() => _players.Values.ToArray();

    public Player GetLocalPlayer()
    {
        return _players.TryGetValue(playerID, out Player _player) ?
            _player :
            null;
    }

    public PlayerData GetCharacterData(string _name) => _charactersData[_name];
    public EnemyData  GetEnemyData(string     _name) => _monstersData[_name];

    private void Awake()
    {
        Instance ??= this;
        DontDestroyOnLoad(gameObject);

        foreach (var _character in characters)
            _charactersData.Add(_character.name, _character);

        foreach (var _monster in monsters)
            _monstersData.Add(_monster.name, _monster);

        currentMap = GameObject.Find("=====SceneObjects=====").transform.Find("prototype").GetComponent<PlayMap>();
    }

    private void Start()
    {
        //TODO : 메서드화 시켜서 다른데에서 초기화할때 불러야함
        var _cinemachineCamera = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera as CinemachineVirtualCamera;
        _cinemachineCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = currentMap.GetBound;
    }

    public void InstantiatePlayer(int _newPlayerID)
    {
        //RaycastAll로 가장 마지막에 충돌한 오브젝트의 위치를 가져옴
        //RaycastNonAlloc이 성능상 더 좋아보이긴 하지만 단발적으로 사용할거라 큰 차이는 없을듯
        Vector2 _mapCenter = currentMap.GetMapSize() / 2;
        
        var _spawnPosition = Physics2D.RaycastAll(_mapCenter, Vector2.down, 200, LayerMask.GetMask("Floor"))[^1].point;
        _spawnPosition.y += 1.5f;
        GameObject _player = Instantiate(playerPrefab, _spawnPosition, Quaternion.identity);
        _player.transform.name = $"Player {_newPlayerID}";

        _player.GetComponent<Player>().clientID = _newPlayerID;
        _players.Add(_newPlayerID, _player.GetComponent<Player>());
        
        if(_newPlayerID == playerID)
            virtualCamera.Follow = _player.transform;
    }

    public void RemovePlayer(int _playerID)
    {
        Destroy(_players[_playerID].gameObject);
        _players.Remove(_playerID);
    }
}