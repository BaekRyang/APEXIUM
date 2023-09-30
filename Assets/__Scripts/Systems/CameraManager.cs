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
        // virtualCamera = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera as CinemachineVirtualCamera;
    }

    private void OnEnable()  => EventBus.Subscribe<PlayMapChangedEvent>(OnMapChanged);
    private void OnDisable() => EventBus.Unsubscribe<PlayMapChangedEvent>(OnMapChanged);

    /// <summary>
    /// 맵 데이터를 받아서 카메라의 바운딩을 설정합니다.
    /// 바운딩 : 카메라가 맵 밖을 비추지 못하도록 설정
    /// </summary>
    /// <param name="_eventData"></param>
    private void OnMapChanged(PlayMapChangedEvent _eventData)
    {
        virtualCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D =
            _eventData.mapData[0].currentMap.GetBound;
    }

    public void SetCameraFollow(Transform _target)
    {
        virtualCamera.Follow = _target;
    }
}