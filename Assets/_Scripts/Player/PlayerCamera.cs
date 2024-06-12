using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera topViewCamera = null;
    [SerializeField] private Camera focusCamera = null;
    [SerializeField] private CameraTarget cameraTarget = null;
    [SerializeField] private float sensitivity = 10f;

    private Transform playerTransform = null;
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
    private void Start()
    {
        playerTransform = PlayerInteraction.Instance.transform;
        UnlockCameraFromTarget();
    }
    public void LockCameraOn(Transform target)
    {
        cameraTarget.SetTargets(playerTransform, target);
    }
    public void UnlockCameraFromTarget()
    {
        cameraTarget.SetTargets(playerTransform, null);
    }
    private void Update()
    {
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
}
