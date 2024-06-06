using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICellDragEnd
{
    public bool IsOverObject(Vector3 mousePos);
    public bool TryDragEnd(InfoCelledUI infoCelledUI);
}
