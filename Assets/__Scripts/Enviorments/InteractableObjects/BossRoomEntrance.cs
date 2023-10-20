using System;
using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossRoomEntrance : InteractableObject
{
    [Inject]                     private MapManager    _mapManager;
    [SerializeField]             private MapType       _rootMapType;
    [Inject]                     private CameraManager _cameraManager;
    [Inject("ShieldBlackBoard")] private Image         _shieldBlackBoard;

    protected override void Initialize()
    {
        destroyAfterInteract = false;
        _rootMapType         = GetComponentInParent<MapData>().currentMap.MapType is MapType.Boss ? MapType.Boss : MapType.Normal;
        DIContainer.Inject(this);

        _mapManager.GetMap(_rootMapType).transform.root.gameObject.GetComponent<MMF_Player>().Initialization();
    }

    protected override void CanNotInteractAction() { }
    protected override bool InteractPredicate(Player _player) => true;

    protected override async void InteractAction(Player _player)
    {
        _player.transform.position = transform.position;

        //플레이어 위치를 문 위치로 이동

        Debug.Log("BossRoomEntrance InteractAction");
        Debug.Log($"_rootMapType : {_rootMapType}");
        _shieldBlackBoard.gameObject.SetActive(true);

        Transform _normalTransform = _mapManager.GetMap(MapType.Normal).transform.root;
        Transform _bossTransform   = _mapManager.GetMap(MapType.Boss).transform.root;

        MMF_Player _thisMMF   = GetComponent<MMF_Player>();
        MMF_Player _normalMMF = _normalTransform.GetComponent<MMF_Player>();
        MMF_Player _bossMMF   = _bossTransform.GetComponent<MMF_Player>();

        MapType _targetMapType  = _rootMapType == MapType.Normal ? MapType.Boss : MapType.Normal;
        MapType _currentMapType = _rootMapType != MapType.Normal ? MapType.Boss : MapType.Normal;
        Debug.Log($"<color=green>targetMapType : {_targetMapType} - _currentMapType : {_currentMapType}</color>");

        PlayMap _targetPlayMap = _mapManager.GetMap(_targetMapType);
        
        //플레이어가 문 중심을 기준으로 얼마나 떨어져있는지 구한다.
        Vector3 _playerOffsetPosition = _player.transform.position - transform.position;
        int     _targetIndex          = _rootMapType == MapType.Normal ? 0 : 1;
        int     _currentIndex         = _rootMapType != MapType.Normal ? 0 : 1;

        Debug.Log($"<color=green>CAMERA : {_targetIndex} - {_currentIndex}</color>");

        //현재 위치의 카메라를 플레이어 위치로 이동
        Camera[] _cameras = _cameraManager.transitionCameras;
        _cameras[_currentIndex].transform.position = _player.transform.position + Vector3.back * 10f;
        Debug.Log($"<color=green> CAM : {_cameras[_currentIndex].name} - {_cameras[_currentIndex].transform.position}</color>");

        //다른 위치의 카메라를 해당 지역의 문 위치 + 오프셋 위치로 이동
        _cameras[_targetIndex].transform.position = _targetPlayMap.bossRoomEntrance.position + _playerOffsetPosition + Vector3.back * 10f;
        Debug.Log($"<color=green> CAM : {_cameras[_targetIndex].name} - {_cameras[_targetIndex].transform.position}</color>");

        _player.Controller.SetControllable(false, true);
        for (int _index = 0; _index < 2; _index++)
        {
            RawImage _rawImage = _cameraManager.transitionTexture[_index];
            Camera   _camera   = _cameras[_index];
            _rawImage.gameObject.SetActive(true);
            _camera.gameObject.SetActive(true);
        }

        _player.transform.position = _targetPlayMap.bossRoomEntrance.position + _playerOffsetPosition;
        
        //플레이어 데이터 업데이트
        _player.currentMap          = _targetPlayMap;
        
        InstantiatePlayerWalkingObject();

        _cameraManager.SetCameraBoundBox(_targetPlayMap);

        _thisMMF.PlayFeedbacks();

        switch (_rootMapType)
        {
            case MapType.Normal:
            {
                _bossMMF.Direction = MMFeedbacks.Directions.TopToBottom;
                _bossMMF.PlayFeedbacks();
                await UniTask.Delay(TimeSpan.FromSeconds(.5f));

                _normalMMF.Direction = MMFeedbacks.Directions.BottomToTop;
                UniTask _task = _normalMMF.PlayFeedbacksUniTask(transform.position);
                await _task;
                break;
            }

            case MapType.Boss:
            {
                _normalMMF.Direction = MMFeedbacks.Directions.TopToBottom;
                _normalMMF.PlayFeedbacks();
                await UniTask.Delay(TimeSpan.FromSeconds(.5f));

                _bossMMF.Direction = MMFeedbacks.Directions.BottomToTop;
                UniTask _task = _bossMMF.PlayFeedbacksUniTask(transform.position);
                await _task;

                break;
            }
        }

        _cameraManager.InvalidateCache();
        _player.Controller.SetControllable(true);

        for (int _index = 0; _index < 2; _index++)
        {
            RawImage _rawImage = _cameraManager.transitionTexture[_index];
            Camera   _camera   = _cameras[_index];
            _rawImage.gameObject.SetActive(false);
            _camera.gameObject.SetActive(false);
        }

        _shieldBlackBoard.gameObject.SetActive(false);
    }

    private void InstantiatePlayerWalkingObject() { }
}