using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternalSettings : MonoBehaviour
{
    [SerializeField] private LayerMask focusedCameraMask;
    [SerializeField] private LayerMask defaultCameraMask;
    public static InternalSettings Get { get; private set; } = null;
    private void Awake()
    {
        if (Get == null) Get = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    public LayerMask FocusedCameraMask => focusedCameraMask;
    public LayerMask DefaultCameraMask => defaultCameraMask;
}
