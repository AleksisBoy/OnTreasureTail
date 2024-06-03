using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("Interactable")]
    [SerializeField] protected Transform interactionPoint = null;
    [SerializeField] protected float interactionDistance = 0.5f;

    protected bool interacted = false;

    protected virtual void Awake()
    {
        if (interactionPoint == null)
        {
            interactionPoint = new GameObject("InteractionPoint").transform;
            interactionPoint.SetParent(transform, false);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.92f, 0.016f, 0.3f);
        Gizmos.DrawSphere(interactionPoint != null ? interactionPoint.position : transform.position, interactionDistance);
    }
}
