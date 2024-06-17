using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera topViewCamera = null;
    [SerializeField] private Camera focusCamera = null;
    [SerializeField] private CameraTarget cameraTarget = null;
    [SerializeField] private float sensitivity = 10f;
    [SerializeField] private float zoomSensitivity = 2f;
    [SerializeField] private float distanceToTarget = 10f;
    [SerializeField] private Vector2 distanceRange = Vector2.zero;

    private Transform playerTransform = null;
    private float DistanceToTarget
    {
        get
        {
            return distanceToTarget;
        }
        set
        {
            value = Mathf.Clamp(value, distanceRange.x, distanceRange.y);
            if (distanceToTarget != value)
            {
                distanceToTarget = value;
                UpdateDistanceToTarget();
            }
        }
    }
    public Camera Current
    {
        get
        {
            if (focusCamera.gameObject.activeSelf) return focusCamera;
            return topViewCamera;
        }
    }
    public Camera FocusCamera => focusCamera;
    public CameraTarget CameraTarget => cameraTarget;
    public static PlayerCamera Instance { get; private set; } = null;
    private void OnValidate()
    {
        distanceToTarget = Mathf.Clamp(distanceToTarget, distanceRange.x, distanceRange.y);
        UpdateDistanceToTarget();
    }
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
    public void Set(Transform playerTransform)
    {
        this.playerTransform = playerTransform;
        UnlockCameraFromTarget();
    }
    public void LockCameraOn(Transform target)
    {
        cameraTarget.SetTargets(playerTransform, target);
        UserInterface.Instance.SetCombatLockTarget(target);
    }
    public void UnlockCameraFromTarget()
    {
        cameraTarget.SetTargets(playerTransform, null);
        UserInterface.Instance.SetCombatLockTarget(null);
    }
    private void Update()
    {
        ZoomInput();
        if (cameraTarget.HasOptionalTarget()) return;

        RotationInput();
    }

    private void RotationInput()
    {
        if (focusCamera.gameObject.activeSelf) return;

        float mouseX = Input.GetAxis("Mouse X");
        if (mouseX != 0f)
        {
            cameraTarget.transform.Rotate(0f, mouseX * Time.deltaTime * sensitivity, 0f);
        }
    }
    private void ZoomInput()
    {
        if (!Input.GetKey(KeyCode.LeftControl)) return;

        float mouseZoom = -Input.mouseScrollDelta.y;
        if (mouseZoom != 0f)
        {
            Debug.Log(mouseZoom);
            DistanceToTarget += mouseZoom * zoomSensitivity;
        }
    }
    private void UpdateDistanceToTarget()
    {
        if (!cameraTarget) return;
        
        Vector3 direction = transform.position - cameraTarget.transform.position;
        direction.Normalize();

        transform.position = cameraTarget.transform.position + direction * distanceToTarget;
    }
}
