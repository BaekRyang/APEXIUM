using System;
using PimDeWitte.UnityMainThreadDispatcher;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PlayerObjectController : IDisposable
{
    [Inject("PlayerPrefab")] private GameObject    playerPrefab;
    [Inject]                 private MapManager    _mapManager;
    [Inject]                 private PlayerManager _playerManager;
    [Inject]                 private PlayData      _playData;

    InjectObj injectObj = new InjectObj();

    public PlayerObjectController()
    {
        EventBus.Subscribe<PlayerEnterEvent>(PlayerEnterEventHandler);
    }

    ~PlayerObjectController()
    {
        ReleaseUnmanagedResources();
    }

    private void PlayerEnterEventHandler(PlayerEnterEvent _eventData)
    {
        Debug.Log("PlayerEnterEventHandler");

        //임시사용

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            injectObj.CheckAndInject(this);

            Debug.Log("PlayerEnterEventHandler : " + _eventData.PlayerID);
            Player _player = InstantiatePlayer(_eventData.PlayerID, _mapManager.GetSpawnLocation());
            _player.SetPlayerData(_playData.characterData); //캐릭터 설정
            Debug.Log($"<color=purple>Player Data Injected</color> - by {_playData.characterData.name}");
            _player.currentMap = _mapManager.GetMap(MapType.Normal);
            _playerManager.AddPlayer(_eventData.PlayerID, _player);
            _player.clientID = _eventData.PlayerID;
        });
    }

    private Player InstantiatePlayer(int _newPlayerID, Vector2 _spawnPosition)
    {
        GameObject _playerObject = GameObject.Instantiate(playerPrefab, _spawnPosition, Quaternion.identity);
        _playerObject.transform.name = $"Player {_newPlayerID}";
        Debug.Log("InstantiatePlayer _player " + _playerObject);
        return _playerObject.GetComponent<Player>();
    }

    public void DestroyPlayer(Player _targetPlayer)
    {
        GameObject.Destroy(_targetPlayer.gameObject);
    }

    private void ReleaseUnmanagedResources()
    {
        Debug.Log("PlayerObjectController Destroyed");
        EventBus.Unsubscribe<PlayerEnterEvent>(PlayerEnterEventHandler);
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }
}