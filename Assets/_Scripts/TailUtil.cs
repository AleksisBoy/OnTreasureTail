using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TailUtil
{
    public static Vector3 PositionFlat(Vector3 position)
    {
        return new Vector3(position.x, 0f, position.z);
    }
}
