using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera mainVirtualCamera;
    public Camera[]                 transitionCameras;

    private void OnEnable()  => EventBus.Subscribe<PlayMapChangedEvent>(OnMapChanged);
    private void OnDisable() => EventBus.Unsubscribe<PlayMapChangedEvent>(OnMapChanged);

    /// <summary>
    /// 맵 데이터를 받아서 카메라의 바운딩을 설정합니다.
    /// 바운딩 : 카메라가 맵 밖을 비추지 못하도록 설정
    /// </summary>
    /// <param name="_eventData"></param>
    private void OnMapChanged(PlayMapChangedEvent _eventData)
    {
        //맵 변경시에는 언제나 메인 맵을 비추고 있으므로 0번 Idx를 사용
        SetCameraBoundBox(_eventData.mapData[0].currentMap);
    }

    public void SetCameraBoundBox(PlayMap _map)
    {
        CinemachineConfiner2D _cinemachineConfiner2D = mainVirtualCamera.GetComponent<CinemachineConfiner2D>();
        _cinemachineConfiner2D.m_BoundingShape2D = _map.GetBound;
        InvalidateCache(_cinemachineConfiner2D);
    }

    public void InvalidateCache(CinemachineConfiner2D _confiner2D = null)
    {
        if (_confiner2D != null)
        {
            _confiner2D.InvalidateCache();
            return;
        }
        CinemachineConfiner2D _cinemachineConfiner2D = mainVirtualCamera.GetComponent<CinemachineConfiner2D>();
        _cinemachineConfiner2D.InvalidateCache();
    }

    public void SetCameraFollow(Transform _target)
    {
        mainVirtualCamera.Follow = _target;
    }
}