using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallaxUpdater : MonoBehaviour
{
    private Dictionary<GameObject, Vector2> _backgrounds;
    
    private void Awake()
    {
        _backgrounds = new Dictionary<GameObject, Vector2>();
        foreach (Transform _child in transform) 
            _backgrounds.Add(_child.gameObject, _child.GetComponent<SpriteRenderer>().sprite.GetResolution());
    }

    //이미지가 작을수록 느리게 움직이고, 크면 빠르게 움직인다.
    //이미지의 크기는 카메라보다 작으면 안됨
    
    
    //플레이어가 맵의 가장자리로 가면
    //배경도 플레이어가 이동한 가장자리 방향으로 이동한다. (동일한 방향)
    private void UpdateParallax()
    {
         
    }
}