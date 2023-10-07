using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class LocalInstaller : MonoBehaviour
{
    public CameraManager cameraManager;
    public MapManager    mapManager;
    public Image         blackBoard;
    public RawImage[]    transitionTexture;
    public Image         shieldBlackBoard;
    
    private void Awake()
    {
        var _container = new DIContainer();
        DIContainer.Local = _container;

        _container.Regist(cameraManager);
        _container.Regist(mapManager);
        _container.Regist(blackBoard, "BlackBoard");
        _container.Regist(transitionTexture);
        _container.Regist(shieldBlackBoard, "ShieldBlackBoard");
    }
}