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

    /// <summary>
    /// 맵 데이터를 받아서 카메라의 바운딩을 설정합니다.
    /// 바운딩 : 카메라가 맵 밖을 비추지 못하도록 설정
    /// </summary>
    /// <param name="_obj"></param>
    private void OnMapChanged(MapChangedEvent _obj)
    {
        virtualCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = _obj.mapData.currentMap.GetBound;
    }
    
    public void SetCameraFollow(Transform _target)
    {
        virtualCamera.Follow = _target;
    }
}