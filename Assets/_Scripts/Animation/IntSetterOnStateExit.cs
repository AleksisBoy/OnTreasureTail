using UnityEngine;

public class IntSetterOnStateExit : StateMachineBehaviour
{
    [SerializeField] private int layer = 0;
    [SerializeField] private float minimumLeaveTime = 0.8f;
    [SerializeField] private string intName = string.Empty;
    [SerializeField] private int intValue = -1;

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (layer != layerIndex) return;
        if (stateInfo.normalizedTime < minimumLeaveTime) return;

        animator.SetInteger(intName, intValue);
    }
}
