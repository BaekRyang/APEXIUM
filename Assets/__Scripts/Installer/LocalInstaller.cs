using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class LocalInstaller : MonoBehaviour
{
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

        _container.Regist(cameraManager);
        _container.Regist(mapManager);
        _container.Regist(blackBoard, "BlackBoard");
        _container.Regist(shieldBlackBoard, "ShieldBlackBoard");
        _container.Regist(volumeProfile);
        _container.Regist(objectPoolManager);
        _container.Regist(itemManager);
    }
}