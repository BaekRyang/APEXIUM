using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static bool       isPlayInSingleMode = true;
    public static MapManager mapManager;

    public Statistics statistics;
    
    public CinemachineVirtualCamera virtualCamera;
    


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        mapManager = GetComponentInChildren<MapManager>();
    }

    private void Start()
    {
        //TODO : 메서드화 시켜서 다른데에서 초기화할때 불러야함
        var _cinemachineCamera = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera as CinemachineVirtualCamera;
        _cinemachineCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = mapManager.GetMap(MapType.Normal).GetBound;
    }



    
}