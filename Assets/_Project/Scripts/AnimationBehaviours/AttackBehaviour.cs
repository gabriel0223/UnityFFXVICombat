using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State Machine Behaviour that communicates when an attack
/// animation starts and when it ends.
/// </summary>
public class AttackBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<PlayerComboController>().OnAttackAnimationStart();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<PlayerComboController>().OnAttackAnimationEnd();
    }
}
