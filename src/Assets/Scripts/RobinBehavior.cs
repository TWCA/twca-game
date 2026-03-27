using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class idleBehavior : StateMachineBehaviour
{
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat("random", Random.Range(0f, 1f));
        
        animator.SetBool("idle", !(
            animator.GetBool("moving") ||
            animator.GetBool("interacting") ||
            animator.GetBool("jumping") ||
            animator.GetBool("petting")));

        if (animator.GetBool("moving") && !(
                animator.GetBool("interacting") ||
                animator.GetBool("jumping") ||
                animator.GetBool("petting")))
            animator.speed = animator.GetFloat("movingSpeed");
        else
            animator.speed = 1;
    }
}