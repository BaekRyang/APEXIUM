using System;
using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;

public class BossRoomEntrance : InteractableObject
{
    [Inject]         private MapManager    _mapManager;
    [SerializeField] private MapType       _rootMapType;
    [Inject]         private CameraManager _cameraManager;
    [Inject]         private RawImage[]    _transitionTexture;

    protected override void Initialize()
    {
        destroyAfterInteract = false;
        _rootMapType         = GetComponentInParent<MapData>().currentMap is BossPlayMap ? MapType.Boss : MapType.Normal;
        DIContainer.Inject(this);

        _mapManager.GetMap(_rootMapType).transform.root.gameObject.GetComponent<MMF_Player>().Initialization();
    }

    protected override async void InteractAction(Player _player)
    {
        Debug.Log("BossRoomEntrance InteractAction");
        Debug.Log($"_rootMapType : {_rootMapType}");


        Transform _normalTransform = _mapManager.GetMap(MapType.Normal).transform.root;
        Transform _bossTransform   = _mapManager.GetMap(MapType.Boss).transform.root;

        MMF_Player _normalMMF = _normalTransform.GetComponent<MMF_Player>();
        MMF_Player _bossMMF   = _bossTransform.GetComponent<MMF_Player>();

        MapType _targetMapType = _rootMapType == MapType.Normal ? MapType.Boss : MapType.Normal;

        //플레이어가 문 중심을 기준으로 얼마나 떨어져있는지 구한다.
        Vector3 _playerOffsetPosition = _player.transform.position - transform.position;
        int     _currentTargetIndex   = _rootMapType == MapType.Normal ? 0 : 1;
        int     _otherTargetIndex     = _rootMapType == MapType.Normal ? 1 : 0;

        //현재 위치한 지역의 카메라를 플레이어 위치로 이동시키고, 다른 지역의 카메라를 해당 지역의 문 위치 + 오프셋 위치로 이동시킨다.
        Camera[] _cameras = _cameraManager.transitionCameras;
        _cameras[_currentTargetIndex].transform.position = _player.transform.position + Vector3.back * 10f;

        _cameras[_otherTargetIndex].transform.position = _mapManager.GetMap(MapType.Boss).bossRoomEntrance.position + _playerOffsetPosition + Vector3.back * 10f;

        _player.Controller.SetControllable(false, true);
        _player.transform.position = _mapManager.GetMap(_targetMapType).bossRoomEntrance.position + _playerOffsetPosition;
        for (int _index = 0; _index < 2; _index++)
        {
            RawImage _rawImage = _transitionTexture[_index];
            Camera   _camera   = _cameras[_index];
            _rawImage.gameObject.SetActive(true);
            _camera.gameObject.SetActive(true);
        }

        _cameraManager.SetCameraBoundBox(_mapManager.GetMap(_targetMapType));

        switch (_rootMapType)
        {
            case MapType.Normal:
            {
                _normalMMF.Direction = MMFeedbacks.Directions.TopToBottom;
                UniTask _task = _normalMMF.PlayFeedbacksUniTask(transform.position);
                _player.Controller.SetControllable(true);
                _bossMMF.Direction = MMFeedbacks.Directions.BottomToTop;
                _task              = _bossMMF.PlayFeedbacksUniTask(transform.position);
                await _task;
                break;
            }

            case MapType.Boss:
            {
                _bossMMF.Direction = MMFeedbacks.Directions.TopToBottom;
                UniTask _task = _bossMMF.PlayFeedbacksUniTask(transform.position);
                _player.Controller.SetControllable(true);
                _normalMMF.Direction = MMFeedbacks.Directions.BottomToTop;
                _task                = _normalMMF.PlayFeedbacksUniTask(transform.position);
                await _task;
                break;
            }
        }

        for (int _index = 0; _index < 2; _index++)
        {
            RawImage _rawImage = _transitionTexture[_index];
            Camera   _camera   = _cameras[_index];
            _rawImage.gameObject.SetActive(false);
            _camera.gameObject.SetActive(false);
        }
    }
}