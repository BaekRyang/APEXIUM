using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera mainVirtualCamera;
    public Camera[]                 transitionCameras;
    public RawImage[]               transitionTexture;

    private void OnEnable()
    {
        EventBus.Subscribe<PlayMapChangedEvent>(OnMapChanged);
        EventBus.Subscribe<ResolutionChanged>(OnResolutionChanged);
    }



    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayMapChangedEvent>(OnMapChanged);
        EventBus.Unsubscribe<ResolutionChanged>(OnResolutionChanged);
    }

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
    
    private void OnResolutionChanged(ResolutionChanged _obj)
    {
        SetTransitionCameraResolution(_obj.resolution.width, _obj.resolution.height);
    }

    public void SetCameraBoundBox(PlayMap _map)
    {
        CinemachineConfiner2D _cinemachineConfiner2D = mainVirtualCamera.GetComponent<CinemachineConfiner2D>();
        _cinemachineConfiner2D.m_BoundingShape2D = _map.Bound;
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

    public void SetTransitionCameraResolution(int _width, int _height)
    {
        for (int _index = 0; _index < 2; _index++)
        {
            Camera _cams = transitionCameras[_index];

            RenderTexture _camsTargetTexture = _cams.targetTexture;

            if (_camsTargetTexture != null) //이미 렌더 텍스쳐가 있다면
            {
                _camsTargetTexture.Release(); //렌더 텍스쳐를 해제하고
                Destroy(_camsTargetTexture);  //렌더 텍스쳐를 삭제
            }

            RenderTexture _targetTexture = new(_width, _height, 16, RenderTextureFormat.ARGBFloat); //HDR 사용(Float)
            _targetTexture.name = $"TransitionTexture {_index + 1}";

            transitionTexture[_index].texture = _targetTexture;

            _targetTexture.Create();

            _cams.targetTexture = _targetTexture;
        }
    }
}

class ResolutionChanged
{
    public readonly SettingData.ResolutionValue resolution;

    public ResolutionChanged(SettingData.ResolutionValue _resolution)
    {
        resolution = _resolution;
    }
}