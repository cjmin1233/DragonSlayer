using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyUtility
{
    public static float AngleBetweenVectors(Vector3 from, Vector3 to)
    {
        from.Normalize();
        to.Normalize();

        float angle = Mathf.Acos(Mathf.Clamp(Vector3.Dot(from, to), -1f, 1f)) * Mathf.Rad2Deg;
        return angle;
    }
}
