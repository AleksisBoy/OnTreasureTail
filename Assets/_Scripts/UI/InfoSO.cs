using System;
using UnityEngine;

public class InfoSO : ScriptableObject
{
    [Header("Info")]
    [SerializeField] protected string infoID = Guid.NewGuid().ToString();
    [SerializeField] protected Vector2Int[] objectSize = null;

    public Vector2Int[] ObjectSize => objectSize;
    private void OnValidate()
    {
        if (infoID == string.Empty) infoID = Guid.NewGuid().ToString();
    }
    public string InfoID => infoID;
}
