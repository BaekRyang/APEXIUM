using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour
{
    public         List<RuntimeAnimatorController> playerAnimations;
    public         List<RuntimeAnimatorController> enemyAnimations;
    private static List<RuntimeAnimatorController> Animations;

    private void Awake()
    {
        Animations = playerAnimations;
        Animations.AddRange(enemyAnimations);
    }

    public static void PlayAnimation(Animator p_animator, string p_animationName)
    {
        p_animationName = p_animator.runtimeAnimatorController.name + "_" + p_animationName;
        Debug.Log(p_animationName);

        p_animator.Play(p_animationName);
    }

    //Animation Controller가져오기
    public static RuntimeAnimatorController GetAnimatorController(string p_animationName)
    {
        RuntimeAnimatorController _animationController = Animations.Find(_animation => _animation.name == p_animationName);
        if (_animationController != null) return _animationController;
        
        Debug.LogError($"AnimationController {p_animationName} not found");
        return null;

    }
}