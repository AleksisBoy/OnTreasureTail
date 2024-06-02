using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraTarget = null;
    [SerializeField] private float sensitivity = 10f;
    private void Update()
    {
        RotationInput();
    }

    private void RotationInput()
    {
        float mouseX = Input.GetAxis("Mouse X");
        if (mouseX != 0f)
        {
            cameraTarget.Rotate(0f, mouseX * Time.deltaTime * sensitivity, 0f);
        }
    }
}
