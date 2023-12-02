using System;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallaxUpdater : MonoBehaviour
{
    [SerializeField] [Inject] private PlayerManager _playerManager;

    private          Dictionary<Transform, Vector2> _backgrounds;
    [Inject] private MapManager                     _mapManager;

    private void Awake()
    {
        _backgrounds = new Dictionary<Transform, Vector2>();
        foreach (Transform _child in transform)
        {
            SpriteRenderer _spriteRenderer = _child.GetComponent<SpriteRenderer>();
            _backgrounds.Add(_child.transform, _spriteRenderer.sprite.GetResolution());
            _spriteRenderer.sortingOrder = _child.GetSiblingIndex() - transform.childCount;
        }
        
        
    }

    private void Start()
    {
        DIContainer.Inject(this);
    }

    private void FixedUpdate()
    {
        if (_playerManager.GetLocalPlayer() != null)
            UpdateParallax(
                _mapManager.GetMap(MapType.Normal).GetMapSize(),
                _playerManager.GetLocalPlayer());
    }

    //이미지가 작을수록 느리게 움직이고, 크면 빠르게 움직인다.
    //이미지의 크기는 카메라보다 작으면 안됨

    //플레이어가 맵의 가장자리로 가면
    //배경도 플레이어가 이동한 가장자리 방향으로 이동한다. (동일한 방향)
    private void UpdateParallax(Vector2 _totalSize, Player _refPlayer)
    {
        foreach ((Transform _backgroundTransform, Vector2 _spriteSize) in _backgrounds)
        {
            _backgroundTransform.position =
                Tools.Remap(
                    _refPlayer.transform.position,
                    new[] { Vector2.zero, _spriteSize },
                    new[] { Vector2.zero, _totalSize }
                );
        }
    }
}