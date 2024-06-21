using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInspection : MonoBehaviour, ITailable
{
    [SerializeField] private float distanceToCamera = 3f;

    private static ObjectInspection Current = null;
    private float step = -1f;
    private bool mouseHeld = false;

    private Camera focusCamera;
    private Vector3 startPosition;
    private Quaternion startRotation;

    private Vector3 tempPosition;
    private Quaternion tempRotation;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    public bool BlockPlayer 
    {
        get => true;
        set => throw new NotImplementedException(); 
    }
    public List<ITailable> Children 
    {
        get => new List<ITailable>();
        set => throw new NotImplementedException(); 
    }
    public static void Clear()
    {
        Current = null;
    }
    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        focusCamera = PlayerCamera.Instance.FocusCamera;
    }
    private void OnMouseOver()
    {
        if (!focusCamera.gameObject.activeSelf) return;
        if (OTTInputModule.Module.GetFocused()) return;

        if (!Current && step < 0f && Input.GetMouseButtonDown(0))
        {
            UIManager.Open(this);
        }
        else if (Current == this && Input.GetMouseButtonDown(0))
        {
            mouseHeld = true;
        }
    }
    private void Update()
    {
        bool dontMove = step < 0f;
        if (dontMove)
        {
            CurrentProcess();
            RotateInput();
        }
        else
        {
            MoveProcess();
        }
    }

    private void RotateInput()
    {
        if (!mouseHeld) return;

        transform.Rotate(PlayerCamera.Instance.FocusCamera_GetForwardDirectionFlat(), InternalSettings.MouseDelta.x * InternalSettings.ObjectInspectionSensitivity, Space.World);
        transform.Rotate(PlayerCamera.Instance.FocusCamera_GetRightDirectionFlat(), InternalSettings.MouseDelta.y * InternalSettings.ObjectInspectionSensitivity, Space.World);

        if (Input.GetMouseButtonUp(0))
        {
            mouseHeld = false;
        }
    }

    private void CurrentProcess()
    {
        if (Current != this || mouseHeld) return;

        if (Input.GetMouseButtonDown(1))
        {
            UIManager.Close(this);
        }
    }

    private void MoveProcess()
    {
        transform.position = Vector3.Lerp(tempPosition, targetPosition, step);
        transform.rotation = Quaternion.Lerp(tempRotation, targetRotation, step);
        step += InternalSettings.ObjectInspectionMoveSpeed * Time.deltaTime;
        if (step >= 1f)
        {
            transform.position = targetPosition;
            transform.rotation = targetRotation;
            step = -1f;
        }
    }

    public void Close()
    {
        Current = null;
        step = 0f;
        tempPosition = transform.position;
        tempRotation = transform.rotation;
        targetPosition = startPosition;
        targetRotation = startRotation;
    }
    public void Open()
    {
        Current = this;
        step = 0f;
        tempPosition = transform.position;
        tempRotation = transform.rotation;
        targetPosition = focusCamera.transform.position + focusCamera.transform.forward * distanceToCamera;
        targetRotation = startRotation;
    }

    public void AddOnClose(Action action)
    {
        throw new NotImplementedException();
    }
    public string GetName()
    {
        return name;
    }
}
