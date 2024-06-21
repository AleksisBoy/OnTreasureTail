using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassUI : MonoBehaviour
{
    [SerializeField] private RectTransform pole = null;

    private PlayerCamera playerCamera;
    private void Start()
    {
        playerCamera = PlayerCamera.Instance;
    }
    private void Update()
    {
        Vector3 direction = playerCamera.GetForwardDirectionFlat();
        Quaternion lookRotation = Quaternion.LookRotation(direction, new Vector3(1f, 0f, 0f));
        pole.eulerAngles = new Vector3(0f, 0f, lookRotation.eulerAngles.y);
    }
}
