using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionAnimation : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("ActionPlaying", true);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsTag("Die")) return;
        animator.SetBool("ActionPlaying", false);
    }
}