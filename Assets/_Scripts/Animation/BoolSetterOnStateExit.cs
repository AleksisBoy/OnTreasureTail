using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoolSetterOnStateExit : StateMachineBehaviour
{
    [SerializeField] private int layer = 0;
    [SerializeField] private float minimumLeaveTime = 0.8f;
    [SerializeField] private string boolName = string.Empty;
    [SerializeField] private bool boolValue = false;

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (layer != layerIndex) return;
        if (stateInfo.normalizedTime < minimumLeaveTime) return;

        animator.SetBool(boolName, boolValue);
    }
}
