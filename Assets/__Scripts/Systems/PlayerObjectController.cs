using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
public class PlayerObjectController
{
    [Inject("PlayerPrefab")] private GameObject playerPrefab;
    public PlayerObjectController()
    {
        DIContainer.Inject(this);
        EventBus.Subscribe<PlayerEnterEvent>(PlayerEnterEventHandler);
    }
    
    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerEnterEvent>(PlayerEnterEventHandler);
    }
    
    private void PlayerEnterEventHandler(PlayerEnterEvent ev)
    {
        Debug.Log("PlayerEnterEventHandler : " + ev.PlayerID);
        InstantiatePlayer(ev.PlayerID, GetInstantiationPosition());
    }
    
    private void InstantiatePlayer(int _newPlayerID, Vector2 _spawnPosition)
    {
        GameObject _player = GameManager.Instantiate(playerPrefab, _spawnPosition, Quaternion.identity);
        _player.transform.name = $"Player {_newPlayerID}";

        _player.GetComponent<Player>().clientID = _newPlayerID;
    }
    
    private Vector2 GetInstantiationPosition()
    {
        Vector2 _mapCenter     = GameManager.mapManager.GetMap(MapType.Normal).GetSize / 2; //TODO : 임시로 맵 중앙으로 스폰
        Vector2 _spawnPosition = Physics2D.RaycastAll(_mapCenter, Vector2.down, 200, LayerMask.GetMask("Floor"))[^1].point;
        _spawnPosition.y += 1.5f;
        return _spawnPosition;
    }

    public void DestroyPlayer(Player _targetPlayer)
    {
        GameObject.Destroy(_targetPlayer.gameObject);
    }
}
