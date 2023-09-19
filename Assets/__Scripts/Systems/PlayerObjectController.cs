using PimDeWitte.UnityMainThreadDispatcher;
using UnityEngine;

public class PlayerObjectController
{
    [Inject("PlayerPrefab")] private GameObject    playerPrefab;
    [Inject]                 private MapManager    _mapManager;
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
            Player _player = InstantiatePlayer(_eventData.PlayerID, _mapManager.GetSpawnLocation());
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

    public void DestroyPlayer(Player _targetPlayer)
    {
        GameObject.Destroy(_targetPlayer.gameObject);
    }
}