using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusCameraINTR : Interactable, IInteractable
{
    [Header("Focus Camera")]
    [SerializeField] private Transform alignObject = null;
    [SerializeField] private float alignTime = 4f;

    private Camera focusCamera = null;
    private Vector3 initialPosition = Vector3.zero;
    private Quaternion initialRotation = Quaternion.identity;
    private float align = 0f;
    private Transform alignTarget = null;

    private void Start()
    {
        AssignToIslandManager();
    }
    private void Update()
    {
        if (focusCamera == null) return;
        if (alignTarget == null)
        {
            FocusedInput();
            return;
        }

        // skip aligning if half way through and clicked any button
        if (align > 0.5f && Input.anyKeyDown) align = 1f;

        if (align > 1f) return;

        AligningProcess();
    }

    private void AligningProcess()
    {
        focusCamera.transform.position = Vector3.Lerp(initialPosition, alignTarget.position, align);
        focusCamera.transform.rotation = Quaternion.Lerp(initialRotation, alignTarget.rotation, align);

        align += Time.deltaTime / alignTime;
        if (align < 1f) return;

        AlignToTarget();
    }

    private void AlignToTarget()
    {
        focusCamera.transform.position = alignTarget.position;
        focusCamera.transform.rotation = alignTarget.rotation;
        if (alignTarget == PlayerInteraction.Instance.GetPlayerCamera().transform)
        {
            // on aligned to back to player camera
            PlayerInteraction.Instance.EnablePlayerComponents(true);
            focusCamera.gameObject.SetActive(false);
            focusCamera = null;
            UIManager.RemoveBlocker(gameObject);
        }
        else
        {
            // on aligned to focus object
            focusCamera.cullingMask = InternalSettings.Get.FocusedCameraMask;
        }
        alignTarget = null;
    }

    private void FocusedInput()
    {
        if (!Input.GetKeyDown(KeyCode.Mouse0)) return;

        focusCamera.cullingMask = InternalSettings.Get.DefaultCameraMask;
        alignTarget = PlayerInteraction.Instance.GetPlayerCamera().transform;
        initialPosition = focusCamera.transform.position;
        initialRotation = focusCamera.transform.rotation;
        align = 0f;
    }

    // IInteractable calls
    public void AssignToIslandManager()
    {
        IslandManager.Instance.AssignInteractable(this);
    }

    public Vector3 GetInteractionPosition()
    {
        return interactionPoint.position;
    }

    public bool HasInteracted()
    {
        return interacted;
    }

    public bool Interact()
    {
        UIManager.AddBlocker(gameObject);
        align = 0f;
        focusCamera = PlayerInteraction.Instance.GetFocusCamera();
        focusCamera.transform.gameObject.SetActive(true);
        initialPosition = focusCamera.transform.position;
        initialRotation = focusCamera.transform.rotation;
        alignTarget = alignObject;
        interacted = true;
        return true;
    }

    public bool InteractionActive()
    {
        return gameObject.activeSelf;
    }
}
