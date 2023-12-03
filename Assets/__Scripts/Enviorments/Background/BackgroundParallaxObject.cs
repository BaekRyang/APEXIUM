using System;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class BackgroundParallaxObject : MonoBehaviour
{
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] public int            layerIndex;

    [Tooltip("카메라의 움직임에 따라 그림이 얼마나 움직일지 결정합니다.")]
    [SerializeField] private float parallaxSpeed = 0;

    [Tooltip("양옆에 추가 이미지를 생성하여 무한루프를 만들지 여부를 결정합니다.")]
    [SerializeField] private bool isLooping = false;

    [SerializeField] private Transform[]              loopingObjects = new Transform[2];
    [Inject]                 CameraManager            _cameraManager;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private float                    _spriteWidth;
    [SerializeField] private int                      _targetIndex;

    private void Start()
    {
        DIContainer.Inject(this);
        _virtualCamera = _cameraManager.mainVirtualCamera;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (isLooping)
        {
            //양 옆에 똑같은 이미지를 생성한다.
            loopingObjects[0] = new GameObject("Looping Object Left").transform;
            loopingObjects[1] = new GameObject("Looping Object Right").transform;

            loopingObjects[0].SetPositionAndRotation(transform.position + Vector3.left  * spriteRenderer.bounds.size.x, Quaternion.identity);
            loopingObjects[1].SetPositionAndRotation(transform.position + Vector3.right * spriteRenderer.bounds.size.x, Quaternion.identity);

            loopingObjects[0].SetParent(transform);
            loopingObjects[1].SetParent(transform);

            var sr0 = loopingObjects[0].AddComponent<SpriteRenderer>();
            var sr1 = loopingObjects[1].AddComponent<SpriteRenderer>();

            sr1.sprite       = sr0.sprite       = spriteRenderer.sprite;
            sr1.sortingOrder = sr0.sortingOrder = spriteRenderer.sortingOrder;

            _spriteWidth = spriteRenderer.bounds.size.x;

            Destroy(spriteRenderer);
        }
    }

    public void UpdatePosition(Vector3 _playerPosition, Vector2 _mapSize)
    {
        Vector3 _cameraPos = _virtualCamera.transform.position;
        if (isLooping)
        {
            //너무 멀면 위치를 변경
            if (Vector2.Distance(_playerPosition, loopingObjects[_targetIndex].position) > _virtualCamera.m_Lens.OrthographicSize * 10)
            {
                loopingObjects[_targetIndex].position = new Vector3(_playerPosition.x, _playerPosition.y);
            }

            Vector2 _playerPosition01 = _playerPosition / _mapSize;

            //parallaxSpeed와 플레이어 움직임에 따라 움직임
            loopingObjects[_targetIndex].position =
                new(_cameraPos.x - parallaxSpeed * 5f * (_playerPosition01.x - .5f),
                    _cameraPos.y);
            
            loopingObjects[1 - _targetIndex].position =
                new(_cameraPos.x - parallaxSpeed * 5f * (_playerPosition01.x - .5f) + _spriteWidth * (_playerPosition01.x > .5f ? 1 : -1),
                    _cameraPos.y);

            //인덱스 변경
            if (Mathf.Abs(_cameraPos.x - loopingObjects[_targetIndex].position.x) > _virtualCamera.m_Lens.OrthographicSize)
                _targetIndex = 1 - _targetIndex;
        }
        else
        {
            if (parallaxSpeed == 0)
            {
                //고정 위치
                transform.position = new Vector3(_cameraPos.x, _cameraPos.y, transform.position.z);
            }
            else
            {
                Vector2 _playerPosition01 = _playerPosition / _mapSize;

                // Debug.Log($"{_playerPosition01.x - .5f},{_playerPosition01.y - .5f}\n" +
                //           $"{_playerPosition01} {10 * (_playerPosition01.x - .5f)},{10 * (_playerPosition01.y - .5f)}");

                //상대 위치에 따라 움직임
                transform.localPosition = new Vector3(_cameraPos.x - parallaxSpeed * 5f * (_playerPosition01.x - .5f),
                                                      _cameraPos.y - parallaxSpeed * (_playerPosition01.y      - .5f),
                                                      transform.position.z);
            }
        }
    }
}