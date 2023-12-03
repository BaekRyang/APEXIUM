using System;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallaxUpdater : MonoBehaviour
{
    [SerializeField] [Inject] private PlayerManager _playerManager;

    private List<BackgroundParallaxObject> _backgroundParallaxObjects = new();

    private Vector2  _mapSize;
    private Player _localPlayer;

    private void Awake()
    {
        foreach (Transform _child in transform)
            _backgroundParallaxObjects.Add(_child.GetComponent<BackgroundParallaxObject>());
    }

    private void OnEnable()
    {
        EventBus.Subscribe<PlayMapChangedEvent>(OnMapChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayMapChangedEvent>(OnMapChanged);
    }

    private void OnMapChanged(PlayMapChangedEvent _obj)
    {
        _mapSize = _obj.mapData[0].currentMap.MapSize;
    }

    private void Start()
    {
        DIContainer.Inject(this);
    }

    private void FixedUpdate()
    {
        _localPlayer ??= _playerManager.GetLocalPlayer();
        if (_localPlayer                    == null) return;
        if (_localPlayer.currentMap.MapType == MapType.Normal)
        {
            foreach (BackgroundParallaxObject _backgroundParallaxObject in _backgroundParallaxObjects)
            {
                _backgroundParallaxObject.UpdatePosition(_localPlayer.transform.position, _mapSize);
            }
            
            
        }
        
    }
}