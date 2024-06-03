using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceRandomlyOnCurve : MonoBehaviour
{
    [SerializeField] private Transform[] objects = null;
    [SerializeField] private bool randomRotation = false;
    [SerializeField] private Vector3 rotationRange = Vector3.zero;
    [SerializeField] private Vector3 scaleRange1 = Vector3.zero;
    [SerializeField] private Vector3 scaleRange2 = Vector3.zero;

    private BezierCurve curve = null;
    private void OnEnable()
    {
        curve = GetComponent<BezierCurve>();
    }
    [ContextMenu("Place Randomly")]
    public void PlaceRandomly()
    {
        if (curve == null)
        {
            if (TryGetComponent<BezierCurve>(out BezierCurve newCurve)) curve = newCurve; 
        }
        if (randomRotation)
        {
            PlaceRandomlyWithRotation();
            return;
        }
        foreach (Transform t in objects)
        {
            t.position = curve.GetRandomPositionOnCurve() + transform.position;
        }
    }
    public void PlaceRandomlyWithRotation()
    {
        foreach(Transform t in objects)
        {
            t.position = curve.GetRandomPositionOnCurve() + transform.position;
            t.rotation = Quaternion.Euler(Random.Range(-rotationRange.x, rotationRange.x), Random.Range(-rotationRange.y, rotationRange.y), Random.Range(-rotationRange.z, rotationRange.z));
            t.localScale = new Vector3(Random.Range(scaleRange1.x, scaleRange2.x), Random.Range(scaleRange1.y, scaleRange2.y), Random.Range(scaleRange1.z, scaleRange2.z));
        }
    }
}
