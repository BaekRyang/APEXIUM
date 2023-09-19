using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;

    private void Start()
    {
        DIContainer.Inject(this);
        
        virtualCamera = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera as CinemachineVirtualCamera;
    }

    private void OnEnable() => EventBus.Subscribe<MapChangedEvent>(OnMapChanged);
    private void OnDisable() => EventBus.Unsubscribe<MapChangedEvent>(OnMapChanged);

    private void OnMapChanged(MapChangedEvent _obj)
    {
        virtualCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = _obj.mapData.currentMap.GetBound;
    }
    
    public void SetCameraFollow(Transform _target)
    {
        virtualCamera.Follow = _target;
    }
}