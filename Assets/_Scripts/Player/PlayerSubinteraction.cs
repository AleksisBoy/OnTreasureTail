using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerSubinteraction : MonoBehaviour
{
    [Header("Subinteraction")]
    [SerializeField] private string itemDependantName = string.Empty;
    [SerializeField] private int animatorLayerIndex = -1;

    protected Animator animator = null;
    public string ItemName => itemDependantName;
    public virtual void Set(Animator animator, params object[] setList)
    {
        this.animator = animator;
    }
    protected virtual void OnEnable()
    {
        if (animatorLayerIndex < 0) return;

        animator?.SetLayerWeight(animatorLayerIndex, 1f);
    }
    protected virtual void OnDisable()
    {
        if (animatorLayerIndex < 0) return;

        animator?.SetLayerWeight(animatorLayerIndex, 0f);
    }
}
