using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternalSettings : MonoBehaviour
{
    [SerializeField] private LayerMask focusedCameraMask;
    [SerializeField] private LayerMask defaultCameraMask;
    [SerializeField] private LayerMask environmentMask;
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
    public LayerMask EnvironmentMask => environmentMask;

    public static void CreateDebugSphere(Vector3 position, float scale)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.position = position;
        go.transform.localScale *= scale;
    }
}
