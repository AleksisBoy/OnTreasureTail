using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "OTT/New Picture Info")]
public class InfoPictureSO : InfoSO
{
    [Header("Picture")]
    [SerializeField] private Texture picture = null;
}
