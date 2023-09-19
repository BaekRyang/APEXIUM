using Cinemachine;
using UnityEngine;


public class LocalInstaller : MonoBehaviour
{
    public CameraManager cameraManager;
    public MapManager MapManager;

    private void Awake()
    {
        var _container = new DIContainer();
        DIContainer.Local = _container;

        _container.Regist(cameraManager);
        _container.Regist(MapManager);
        
       
    }
}
