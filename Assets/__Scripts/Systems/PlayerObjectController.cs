using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using PimDeWitte.UnityMainThreadDispatcher;
using UnityEngine;

public class PlayerObjectController
{
    [Inject]                 private MapManager    _mapManager;
    [Inject("PlayerPrefab")] private GameObject    playerPrefab;
    [Inject]                 private PlayerManager _playerManager;

    InjectObj injectObj = new InjectObj();

    public PlayerObjectController()
    {
        EventBus.Subscribe<PlayerEnterEvent>(PlayerEnterEventHandler);
    }

    ~PlayerObjectController()
    {
        EventBus.Unsubscribe<PlayerEnterEvent>(PlayerEnterEventHandler);
    }

    private void PlayerEnterEventHandler(PlayerEnterEvent _eventData)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            injectObj.CheckAndInject(this);

            Debug.Log("PlayerEnterEventHandler : " + _eventData.PlayerID);
            Player _player = InstantiatePlayer(_eventData.PlayerID, GetInstantiationPosition());
            _playerManager.AddPlayer(_eventData.PlayerID, _player);
            _player.clientID = _eventData.PlayerID;
        });
    }

    private Player InstantiatePlayer(int _newPlayerID, Vector2 _spawnPosition)
    {
        GameObject _playerObject    = GameObject.Instantiate(playerPrefab, _spawnPosition, Quaternion.identity);
        _playerObject.transform.name = $"Player {_newPlayerID}";
        Debug.Log("InstantiatePlayer _player " + _playerObject);
        return _playerObject.GetComponent<Player>();
    }

    private Vector2 GetInstantiationPosition()
    {
        Vector2 _mapCenter     = _mapManager.GetMap(MapType.Normal).GetSize / 2; //TODO : 임시로 맵 중앙으로 스폰
        Vector2 _spawnPosition = Physics2D.RaycastAll(_mapCenter, Vector2.down, 200, LayerMask.GetMask("Floor"))[^1].point;
        _spawnPosition.y += 1.5f;
        return _spawnPosition;
    }

    public void DestroyPlayer(Player _targetPlayer)
    {
        GameObject.Destroy(_targetPlayer.gameObject);
    }
}