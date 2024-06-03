using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "OTT/New Text Info")]
public class InfoTextSO : InfoSO
{
    [Header("Text")]
    [SerializeField] private string text = string.Empty;

    public string Text => text;
}
