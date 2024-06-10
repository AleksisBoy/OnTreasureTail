using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera topViewCamera = null;
    [SerializeField] private Camera focusCamera = null;
    [SerializeField] private Transform cameraTarget = null;
    [SerializeField] private float sensitivity = 10f;

    public Camera Current
    {
        get
        {
            if (focusCamera.gameObject.activeSelf) return focusCamera;
            return topViewCamera;
        }
    }
    public Camera FocusCamera => focusCamera;
    public static PlayerCamera Instance { get; private set; } = null;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        focusCamera.gameObject.SetActive(false);
    }
    private void Update()
    {
        RotationInput();
    }

    private void RotationInput()
    {
        if (focusCamera.gameObject.activeSelf) return;

        float mouseX = Input.GetAxis("Mouse X");
        if (mouseX != 0f)
        {
            cameraTarget.Rotate(0f, mouseX * Time.deltaTime * sensitivity, 0f);
        }
    }
}
