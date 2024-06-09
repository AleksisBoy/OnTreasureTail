using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerSubinteraction : MonoBehaviour
{
    [SerializeField] private string itemDependantName = string.Empty;
    [SerializeField] protected Animator animator = null;
    [SerializeField] private int animatorLayerIndex = -1;

    public string ItemName => itemDependantName;
    public virtual void Set(params object[] setList)
    {

    }
    protected virtual void OnEnable()
    {
        animator.SetLayerWeight(animatorLayerIndex, 1f);
    }
    protected virtual void OnDisable()
    {
        animator.SetLayerWeight(animatorLayerIndex, 0f);
    }
}
