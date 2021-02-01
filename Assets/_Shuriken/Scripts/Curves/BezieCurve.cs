using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezieCurve : MonoBehaviour
{
    /// <summary>
    ///  Как построить кривую безье
    /// </summary>
    /// <param name="p0"> Start Point </param>
    /// <param name="p1"> Start Point Anchor </param>
    /// <param name="p2"> End Point Anchor</param>
    /// <param name="p3">End Point</param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector3 GetPointOnBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1f - t;
        float t2 = t * t;
        float u2 = u * u;
        float u3 = u2 * u;
        float t3 = t2 * t;

        Vector3 result =
            (u3) * p0 +
            (3f * u2 * t) * p1 +
            (3f * u * t2) * p2 +
            (t3) * p3;

        return result;
    }
}
