using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWaitForAnimation : CustomYieldInstruction
{
    Animator m_animator;
    int m_lastStateHash = 0;
    int m_layerNo = 0;

    public PlayerWaitForAnimation(Animator animator, int layerNo)
    {
        //Init(animator, layerNo, animator.GetCurrentAnimatorStateInfo(layerNo).nameHash);
        //animator.Animator
        Init(animator, layerNo, animator.GetCurrentAnimatorStateInfo(0).fullPathHash);
    }

    void Init(Animator animator, int layerNo, int hash)
    {
        m_layerNo = layerNo;
        m_animator = animator;
        m_lastStateHash = hash;
    }

    public override bool keepWaiting
    {
        get
        {
            var currentAnimatorState = m_animator.GetCurrentAnimatorStateInfo(m_layerNo);
            return currentAnimatorState.fullPathHash == m_lastStateHash &&
                (currentAnimatorState.normalizedTime < 1);
        }
    }
}
