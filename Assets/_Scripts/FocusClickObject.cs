using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class FocusClickObject : MonoBehaviour
{
    [SerializeField] private UnityEvent onMouseDown = null;

    private void OnMouseDown()
    {
        if (PlayerCamera.Instance.FocusCamera.gameObject.activeSelf && !OTTInputModule.Module.GetFocused())
        {
            onMouseDown?.Invoke();
        }
    }
}
