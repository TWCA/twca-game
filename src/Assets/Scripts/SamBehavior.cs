using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamBehavior : StateMachineBehaviour
{
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // animator.SetFloat("random", Random.Range(0f, 1f));

        animator.SetBool("idle", !(
            animator.GetBool("walk") ||
            animator.GetBool("pet")));

        if (animator.GetBool("walk") && !(
                animator.GetBool("idle") ||
                animator.GetBool("pet")))
            // animator.speed = animator.GetFloat("movingSpeed");
            animator.speed = 1;
        else
            animator.speed = 1;
    }
}