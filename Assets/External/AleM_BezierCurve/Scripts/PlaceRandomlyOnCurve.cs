using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceRandomlyOnCurve : MonoBehaviour
{
    [SerializeField] private Transform[] objects = null;
    [SerializeField] private bool randomRotationY = false;

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
        if (randomRotationY)
        {
            PlaceRandomlyWithRotationY();
            return;
        }
        foreach (Transform t in objects)
        {
            t.position = curve.GetRandomPositionOnCurve() + transform.position;
        }
    }
    public void PlaceRandomlyWithRotationY()
    {
        foreach(Transform t in objects)
        {
            t.position = curve.GetRandomPositionOnCurve() + transform.position;
            t.rotation = Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f);
        }
    }
}
