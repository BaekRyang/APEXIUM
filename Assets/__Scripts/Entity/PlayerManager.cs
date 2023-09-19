using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class PlayerManager
{
    private PlayerObjectController _playerObjectController;

    public int playerID = -1;

    [DoNotSerialize] private Dictionary<int, Player> _players = new();

    public PlayerManager() { }

    public void Initialize()
    {
        _playerObjectController = new();
    }

    public void SetSpawner(PlayerObjectController _objectController)
    {
        _playerObjectController = _objectController;
    }

    public Player GetRandomPlayer()
    {
        return _players.Count switch
        {
            <= 0 => null,
            _    => _players[Random.Range(0, _players.Count)]
        };
    }

    public IEnumerable<Player> GetPlayersArray() => _players.Values.ToArray();

    public bool IsLocalPlayer(int _playerID) => _playerID == playerID;

    public Player GetLocalPlayer()
    {
        return GameManager.isPlayInSingleMode ?
            _players.TryGetValue(0, out Player _localPlayer) ?
                _localPlayer :
                null :
            _players.TryGetValue(playerID, out Player _player) ?
                _player :
                null;
    }

    public void AddPlayer(int _newPlayerID, Player _player)
    {
        _players.Add(_newPlayerID, _player);

        Debug.Log("Now Players : " + _players.Count);
    }

    public void RemovePlayer(int _playerID)
    {
        //대상 플레이어가 있으면
        if (_players.TryGetValue(_playerID, out Player _targetPlayer))
        {
            _playerObjectController.DestroyPlayer(_targetPlayer);
            _players.Remove(_playerID);
        }

        Debug.Log($"{_playerID} is Disconnected");

        if (_players.Count != 0)
        {
            foreach ((int _key, Player _) in _players)
                Debug.Log($"Remaining Player ID: {_key}");
        }
        else
            Debug.Log("All Players are Disconnected");
    }
    
    
}