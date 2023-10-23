using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class LocalInstaller : MonoBehaviour
{
    public GameObject playerPrefab;
    public Client     client;
    
    public CameraManager     cameraManager;
    public MapManager        mapManager;
    public Image             blackBoard;
    public Image             shieldBlackBoard;
    public VolumeProfile     volumeProfile;
    public ObjectPoolManager objectPoolManager;
    public ItemManager       itemManager;
    
    private void Awake()
    {
        var _container = new DIContainer();
        DIContainer.Local = _container;
        
        _container.Register(playerPrefab, "PlayerPrefab");
        _container.Register(new PlayerManager().Initialize());
        _container.Register(client);

        _container.Register(cameraManager);
        _container.Register(mapManager);
        _container.Register(blackBoard, "BlackBoard");
        _container.Register(shieldBlackBoard, "ShieldBlackBoard");
        _container.Register(volumeProfile);
        _container.Register(objectPoolManager);
        _container.Register(itemManager);
    }
}