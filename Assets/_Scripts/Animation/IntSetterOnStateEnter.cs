using UnityEngine;

public class IntSetterOnStateEnter : StateMachineBehaviour
{
    [SerializeField] private int layer = 0;
    [SerializeField] private string intName = string.Empty;
    [SerializeField] private int intValue = -1;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (layer != layerIndex) return;

        animator.SetInteger(intName, intValue);
    }
}
