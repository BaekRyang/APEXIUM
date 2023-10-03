using System;
using UnityEngine;

public class BossRoomEntrance : InteractableObject
{
    [Inject]         private MapManager _mapManager;
    [SerializeField] private MapType    _rootMapType;

    protected override void Initialize()
    {
        destroyAfterInteract = false;
        _rootMapType         = GetComponentInParent<MapData>().currentMap is BossPlayMap ? MapType.Boss : MapType.Normal;
        DIContainer.Inject(this);
    }

    protected override void InteractAction()
    {
        Debug.Log("BossRoomEntrance InteractAction");
        Debug.Log($"_rootMapType : {_rootMapType}");

        switch (_rootMapType)
        {
            case MapType.Normal:
                _mapManager.GetMap(MapType.Boss).transform.root.gameObject.SetActive(true);
                _mapManager.GetMap(MapType.Normal).transform.root.gameObject.SetActive(false);
                break;
            case MapType.Boss:
                _mapManager.GetMap(MapType.Boss).transform.root.gameObject.SetActive(false);
                _mapManager.GetMap(MapType.Normal).transform.root.gameObject.SetActive(true);
                break;
        }
    }
}