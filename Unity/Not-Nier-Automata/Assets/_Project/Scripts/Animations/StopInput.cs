using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stops a <see cref="Pawn"/> from moving while this animation plays.
/// </summary>
public class StopInput : StateMachineBehaviour
{
    Pawn pawn;

    /// <summary>
    /// How far through the animation the script should stop blocking input
    /// </summary>
    [Range(0f,1f)]
    public float returnControlAt = 1f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        pawn = animator.GetComponent<Pawn>();
        
        if (pawn)
        {
            pawn.stopMovement = true;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= returnControlAt)
        {
            if (pawn)
            {
                pawn.stopMovement = false;
            }
            
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (pawn)
        //{
        //    pawn.stopMovement = false;
        //}
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
