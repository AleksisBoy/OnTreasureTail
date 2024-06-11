using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [SerializeField] private Transform target = null;
    [SerializeField] private Transform optionalTarget = null;
    [SerializeField] private float followSpeed = 4f;

    private void Update()
    {
        if (optionalTarget)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation((TailUtil.PositionFlat(optionalTarget.position) - TailUtil.PositionFlat(transform.position)).normalized), 10f * Time.deltaTime);
            
            transform.position = Vector3.Lerp(transform.position, (optionalTarget.position + target.position) / 2f, Time.deltaTime * followSpeed);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
        }
    }
    public void SetTarget(Transform target)
    {
        this.target = target;   
    }
    public void SetTargets(Transform target, Transform optionalTarget)
    {
        this.target = target;   
        this.optionalTarget = optionalTarget;
    }
    public bool HasOptionalTarget()
    {
        return optionalTarget != null;
    }
}
