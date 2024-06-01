using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] private Transform target = null;
    [SerializeField] private float followSpeed = 4f;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
    }
}
