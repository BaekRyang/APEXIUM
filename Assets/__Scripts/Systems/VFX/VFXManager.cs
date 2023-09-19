using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public GameObject                  serializedVfxPrefab;
    public RuntimeAnimatorController[] serializedAnimations;

    public static GameObject                                    vfxPrefab;
    public static Dictionary<string, RuntimeAnimatorController> animations = new();

    private void Awake()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        vfxPrefab = serializedVfxPrefab;

        foreach (var _animation in serializedAnimations)
            animations.Add(_animation.name, _animation);
    }

    public static async void PlayVFX(string _vfxName, Vector3 _position, int _flipSprite)
    {
        Animator _vfx = Instantiate(vfxPrefab, _position, Quaternion.identity).GetComponent<Animator>();
        _vfx.transform.localScale      = new Vector3(-_flipSprite, 1, 1);                      //플레이어가 바라보는 방향에 따라 스프라이트 방향 전환
        _vfx.runtimeAnimatorController = animations[_vfxName];                                 //애니메이션 재생
        await UniTask.Delay(TimeSpan.FromSeconds(_vfx.GetCurrentAnimatorStateInfo(0).length)); //애니메이션 재생이 끝날 때까지 대기
        Destroy(_vfx.gameObject);                                                              //애니메이션 재생이 끝나면 오브젝트 삭제
    }
}